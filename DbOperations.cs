
using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

using Dapper.EncapsulationsAndBatching.Types;

namespace Dapper.EncapsulationsAndBatching;

public static class DbOperations
{
	public static async Task ExecuteAsync (
		this IDbConnection dbConnection,
		params IDbOperation[] dbOperations)
	{
		if (dbConnection == null) throw new ArgumentNullException(nameof(dbConnection));
		if (dbOperations == null) throw new ArgumentNullException(nameof(dbOperations));

		if (dbOperations.Length == 0) return;

		if (dbOperations.Length == 1)
			await dbOperations[0].ExecuteAloneAsync(dbConnection);
		else
			await _ExecuteMultipleAsync(dbOperations, dbConnection);
	}

	private static async Task _ExecuteMultipleAsync (
		IDbOperation[] dbOperations,
		IDbConnection dbConnection)
	{
		var combinedQueriesAndParameters = CombineQueriesAndParameters(dbOperations);

		using var multiQuery =
			await dbConnection
			.QueryMultipleAsync(
				combinedQueriesAndParameters.SQL,
				combinedQueriesAndParameters.Parameters);

		for (int q = 0; q < dbOperations.Length; q++)
			await dbOperations[q].ReadResultAsync(multiQuery);
	}

	private static CombinedSQLAndParameters CombineQueriesAndParameters (
		IDbOperation[] dbOperations)
	{
		string combinedSQL = string.Empty;
		var combinedParameters = new Dictionary<string, object?>();
		ushort duplicateParameterModifier = 0;

		for (ushort o = 0; o < dbOperations.Length; o++)
		{
			if (dbOperations[o] == null)
				throw new ArgumentNullException(nameof(dbOperations));

			var dbOperation = dbOperations[o];

			var sql = dbOperation.SQL;

			if (dbOperation.Parameters is not null)
			{
				var parameters = dbOperation.Parameters.GetType().GetProperties();

				for (ushort p = 0; p < parameters.Length; p++)
				{
					var parameter = parameters[p];

					string parameterName = parameter.Name;
					object? value = parameter.GetValue(dbOperation.Parameters);

					if (combinedParameters.ContainsKey(parameterName))
					{
						parameterName += duplicateParameterModifier++.ToString();
						sql = sql.Replace($"@{parameter.Name}", $"@{parameterName}");
					}

					combinedParameters.Add(parameterName, value);
				}
			}

			combinedSQL += sql[sql.Length-1] == ';' ? sql : sql + ';';
		}

		return new CombinedSQLAndParameters(combinedSQL, combinedParameters);
	}

	private readonly struct CombinedSQLAndParameters
	{
		public string SQL { get; }
		public Dictionary<string, object?> Parameters { get; }

		public CombinedSQLAndParameters (
			string sql,
			Dictionary<string, object?> parameters)
		{
			SQL = sql;
			Parameters = parameters;
		}
	}
}
