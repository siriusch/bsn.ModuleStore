using System;
using System.Data.SqlClient;

namespace bsn.ModuleStore.Mapper {
	internal sealed class ConnectionProvider: IConnectionProvider {
		private readonly string connectionString;
		private readonly string schemaName;

		public ConnectionProvider(string connectionString, string schemaName) {
			if (string.IsNullOrEmpty(connectionString)) {
				throw new ArgumentNullException("connectionString");
			}
			if (string.IsNullOrEmpty(schemaName)) {
				throw new ArgumentNullException("schemaName");
			}
			this.connectionString = connectionString;
			this.schemaName = schemaName;
		}

		public string SchemaName {
			get {
				return schemaName;
			}
		}

		public SqlConnection GetConnection() {
			return new SqlConnection(connectionString);
		}

		SqlTransaction IConnectionProvider.GetTransaction() {
			return null;
		}
	}
}