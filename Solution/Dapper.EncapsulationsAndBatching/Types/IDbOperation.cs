
using System.Data;
using System.Threading.Tasks;

namespace Dapper.EncapsulationsAndBatching.Types
{
	public interface IDbOperation
	{
		string SQL { get; }
		object? Parameters { get; }

		Task ExecuteAloneAsync (IDbConnection dbConnection);
		Task ReadResultAsync (SqlMapper.GridReader multiQuery);
	}
}
