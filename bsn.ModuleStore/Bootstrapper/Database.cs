using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace bsn.ModuleStore.Bootstrapper {
	public class Database {
		private readonly string connectionString;

		public Database(string connectionString) {
			this.connectionString = connectionString;
		}

		public DatabaseType GetDatabaseType() {
			using (SqlConnection connection = new SqlConnection(connectionString)) {
				string dbName = connection.Database;
				connection.Open();
				using (SqlCommand command = connection.CreateCommand()) {
					command.CommandText = "SELECT DB_ID(@dbname)";
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@dbname", dbName);
					object dbId = command.ExecuteScalar();
					if (dbId == DBNull.Value) {
						return DatabaseType.None;
					}
				}
				Debug.Assert(connection.Database.Equals(dbName, StringComparison.OrdinalIgnoreCase));
				using (SqlCommand command = connection.CreateCommand()) {
					command.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME='ModuleStore'";
					command.CommandType = CommandType.Text;
					if (Convert.ToBoolean(command.ExecuteScalar())) {
						return DatabaseType.ModuleStore;
					}
				}
				using (SqlCommand command = connection.CreateCommand()) {
					command.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME <> 'sysdiagrams'";
					command.CommandType = CommandType.Text;
					if (Convert.ToInt32(command.ExecuteScalar()) == 0) {
						return DatabaseType.Empty;
					}
				}
				return DatabaseType.Other;
			}
		}
	}
}
