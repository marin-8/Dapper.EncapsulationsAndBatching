
using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Dapper.EncapsulationsAndBatching.Types;

public abstract class QueryList<T> : IDbOperation
{
	public string SQL { get; }
	public object? Parameters { get; }
	public IEnumerable<T>? Result { get; private set; }

	public QueryList (string sql, object? parameters=null)
	{
		SQL = sql ?? throw new ArgumentNullException(nameof(sql));
		Parameters = parameters;
	}

	public async Task ExecuteAloneAsync (IDbConnection dbConnection)
	{
		if (dbConnection == null) throw new ArgumentNullException(nameof(dbConnection));

		Result = await dbConnection.QueryAsync<T>(SQL, Parameters);
	}

	public async Task ReadResultAsync (SqlMapper.GridReader multiQuery)
	{
		if (multiQuery == null) throw new ArgumentNullException(nameof(multiQuery));

		Result = await multiQuery.ReadAsync<T>();
	}
}
