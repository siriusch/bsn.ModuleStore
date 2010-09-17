using System.Data.SqlClient;

namespace bsn.ModuleStore.Mapper {
	public interface IConnectionProvider {
		string SchemaName {
			get;
		}
		SqlConnection GetConnection();
		SqlTransaction GetTransaction();
	}
}