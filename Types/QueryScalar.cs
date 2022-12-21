
using System;
using System.Data;
using System.Threading.Tasks;

namespace Dapper.EncapsulationsAndBatching.Types;

public abstract class QueryScalar<T> : IDbOperation
{
	public string SQL { get; }
	public object? Parameters { get; }
	public T? Result { get; private set; }

	public QueryScalar (string sql, object? parameters=null)
	{
		if (sql == null) throw new ArgumentNullException(nameof(sql));

		SQL = sql[sql.Length-1] == ';' ? sql : sql + ';';
		Parameters = parameters;
	}

	public async Task ExecuteAloneAsync (IDbConnection dbConnection)
	{
		if (dbConnection == null) throw new ArgumentNullException(nameof(dbConnection));

		Result = await dbConnection.ExecuteScalarAsync<T>(SQL, Parameters);
	}

	public async Task ReadResultAsync (SqlMapper.GridReader multiQuery)
	{
		if (multiQuery == null) throw new ArgumentNullException(nameof(multiQuery));

		Result = await multiQuery.ReadSingleAsync<T>();
	}
}
