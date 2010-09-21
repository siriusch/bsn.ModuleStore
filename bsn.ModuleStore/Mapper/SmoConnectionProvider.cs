using System;
using System.Data;
using System.Data.SqlClient;

using Microsoft.SqlServer.Management.Common;

namespace bsn.ModuleStore.Mapper {
	public sealed class SmoConnectionProvider: IConnectionProvider, IDisposable {
		private readonly string schemaName;
		private readonly ServerConnection serverConnection;
		private readonly SqlConnection connection;

		public SmoConnectionProvider(string connectionString, string schemaName): this(new SqlConnection(connectionString), schemaName) {}

		public SmoConnectionProvider(SqlConnection connection, string schemaName) {
			if (connection == null) {
				throw new ArgumentNullException("connection");
			}
			if (connection.State != ConnectionState.Closed) {
				throw new ArgumentException("The connection must be closed", "connection");
			}
			if (String.IsNullOrEmpty(schemaName)) {
				throw new ArgumentNullException("schemaName");
			}
			this.schemaName = schemaName;
			try {
				connection.Open();
				serverConnection = new ServerConnection(connection);
			} catch {
				connection.Dispose();
				throw;
			}
			this.connection = connection;
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