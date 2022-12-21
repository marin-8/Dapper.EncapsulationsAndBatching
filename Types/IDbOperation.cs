
using System.Data;
using System.Threading.Tasks;

namespace Dapper.EncapsulationsAndBatching.Types;

public interface IDbOperation
{
	public string SQL { get; }
	public object? Parameters { get; }

	public Task ExecuteAloneAsync (IDbConnection dbConnection);
	public Task ReadResultAsync (SqlMapper.GridReader multiQuery);
}
