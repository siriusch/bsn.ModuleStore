using System;
using System.Data.SqlClient;

using Microsoft.SqlServer.Management.Common;

namespace bsn.ModuleStore.Mapper {
	internal sealed class SmoConnectionProvider: IConnectionProvider, IDisposable {
		private readonly string schemaName;
		private readonly ServerConnection serverConnection;
		private readonly SqlConnection connection;

		public SmoConnectionProvider(string connectionString, string schemaName) {
			if (string.IsNullOrEmpty(connectionString)) {
				throw new ArgumentNullException("connectionString");
			}
			if (String.IsNullOrEmpty(schemaName)) {
				throw new ArgumentNullException("schemaName");
			}
			this.schemaName = schemaName;
			connection = new SqlConnection(connectionString);
			try {
				connection.Open();
				serverConnection = new ServerConnection(connection);
			} catch {
				connection.Dispose();
				connection = null;
				throw;
			}
		}

		public string SchemaName {
			get {
				return schemaName;
			}
		}

		public string DatabaseName {
			get {
				return connection.Database;
			}
		}

		public ServerConnection ServerConnection {
			get {
				return serverConnection;
			}
		}

		public SqlConnection GetConnection() {
			return connection;
		}

		SqlTransaction IConnectionProvider.GetTransaction() {
			return null;
		}

		public void Dispose() {
			if (connection != null) {
				connection.Dispose();
			}
		}
	}
}