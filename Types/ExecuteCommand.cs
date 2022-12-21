
using System;
using System.Data;
using System.Threading.Tasks;

namespace Dapper.EncapsulationsAndBatching.Types;

public abstract class ExecuteCommand : IDbOperation
{
	public string SQL { get; }
	public object? Parameters { get; }

	public ExecuteCommand (string sql, object? parameters=null)
	{
		if (sql == null) throw new ArgumentNullException(nameof(sql));

		SQL = sql[sql.Length-1] == ';' ? sql : sql + ';';
		Parameters = parameters;
	}

	public async Task ExecuteAloneAsync (IDbConnection dbConnection)
	{
		if (dbConnection == null) throw new ArgumentNullException(nameof(dbConnection));

		_ = await dbConnection.ExecuteAsync(SQL, Parameters);
	}

	public Task ReadResultAsync (SqlMapper.GridReader multiQuery)
	{
		return Task.CompletedTask;
	}
}
