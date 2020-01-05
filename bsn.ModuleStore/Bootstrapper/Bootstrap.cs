// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

using NLog;

namespace bsn.ModuleStore.Bootstrapper {
	internal static class Bootstrap {
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private static void CreateModuleStoreSchema(ModuleDatabase database, string dbName, ModuleInstanceCache cache) {
			database.ManagementConnectionProvider.BeginTransaction();
			var commit = false;
			try {
				database.CreateInstanceDatabaseSchema(cache.AssemblyInfo.Inventory, "ModuleStore");
				using (var command = database.ManagementConnectionProvider.GetConnection().CreateCommand()) {
					command.Transaction = database.ManagementConnectionProvider.GetTransaction();
					command.CommandType = CommandType.Text;
					command.CommandText = "INSERT [ModuleStore].[tblModule] ([uidAssemblyGuid], [sSchema], [sAssemblyName], [binSetupHash], [iUpdateVersion]) VALUES (@uidAssemblyGuid, 'ModuleStore', @sAssemblyName, @binSetupHash, @iUpdateVersion)";
					command.Parameters.AddWithValue("@uidAssemblyGuid", cache.AssemblyInfo.AssemblyGuid);
					command.Parameters.AddWithValue("@sAssemblyName", cache.AssemblyInfo.Assembly.FullName);
					command.Parameters.AddWithValue("@binSetupHash", cache.AssemblyInfo.Inventory.GetInventoryHash(database.ManagementConnectionProvider.Engine));
					command.Parameters.AddWithValue("@iUpdateVersion", cache.AssemblyInfo.Inventory.UpdateVersion);
					command.ExecuteNonQuery();
				}
				commit = true;
			} finally {
				database.ManagementConnectionProvider.EndTransaction(commit);
			}
			Trace.WriteLine($"Installed ModuleStore in database {dbName}");
		}

		internal static DatabaseType GetDatabaseType(string connectionString, out string dbName) {
			using (var connection = new SqlConnection(connectionString)) {
				dbName = connection.Database;
				connection.Open();
				return GetDatabaseType(connection, dbName);
			}
		}

		internal static DatabaseType GetDatabaseType(SqlConnection connection) {
			if (connection == null) {
				throw new ArgumentNullException(nameof(connection));
			}
			return GetDatabaseType(connection, connection.Database);
		}

		private static DatabaseType GetDatabaseType(SqlConnection connection, string dbName) {
			using (var command = connection.CreateCommand()) {
				command.CommandText = "SELECT DB_ID(@dbname)";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@dbname", dbName);
				var dbId = command.ExecuteScalar();
				if (dbId == DBNull.Value) {
					return DatabaseType.None;
				}
			}
			Debug.Assert(connection.Database.Equals(dbName, StringComparison.OrdinalIgnoreCase));
			using (var command = connection.CreateCommand()) {
				command.CommandText = "SELECT COUNT(*) FROM [INFORMATION_SCHEMA].[SCHEMATA] WHERE [SCHEMA_NAME]='ModuleStore'";
				command.CommandType = CommandType.Text;
				if (Convert.ToBoolean(command.ExecuteScalar())) {
					return DatabaseType.ModuleStore;
				}
			}
			using (var command = connection.CreateCommand()) {
				command.CommandText = "SELECT COUNT(*) FROM [INFORMATION_SCHEMA].[TABLES] WHERE [TABLE_NAME] <> 'sysdiagrams'";
				command.CommandType = CommandType.Text;
				if (Convert.ToInt32(command.ExecuteScalar()) == 0) {
					return DatabaseType.Empty;
				}
			}
			return DatabaseType.Other;
		}

		public static void InitializeModuleStore(ModuleDatabase database) {
			var commit = false;
			database.ManagementConnectionProvider.BeginTransaction();
			try {
				log.Trace("Got ModuleStore proxy");
				var cache = database.GetModuleInstanceCache(typeof(IModules).Assembly);
				switch (GetDatabaseType(database.ConnectionString, out var dbName)) {
				case DatabaseType.Other:
					log.Error("The database {dbName} is not a ModuleStore database", dbName);
					throw new InvalidOperationException($"The database {dbName} is not a ModuleStore database");
				case DatabaseType.Empty:
					if (!database.AutoUpdate) {
						log.Error("The database {dbName} is empty, but ModuleStore is not allowed to initialize it", dbName);
						throw new InvalidOperationException($"The database {dbName} is empty");
					}
					log.Debug("Create ModuleStore schema start");
					CreateModuleStoreSchema(database, dbName, cache);
					log.Debug("Create ModuleStore schema end");
					break;
				case DatabaseType.ModuleStore:
					if (database.AutoUpdate || database.ForceUpdateCheck) {
						log.Debug("Update ModuleStore schema start");
						UpdateModuleStoreSchema(database, dbName, cache);
						log.Debug("Update ModuleStore schema end");
					}
					break;
				case DatabaseType.None:
					log.Error("The database {dbName} does not exist", dbName);
					throw new InvalidOperationException($"The database {dbName} does not exist");
				}
				commit = true;
			} catch (Exception ex) {
				log.Error(ex, "ModuleStore initialization failed");
				throw;
			} finally {
				database.ManagementConnectionProvider.EndTransaction(commit);
			}
			log.Trace("ModuleStore DB initialized");
		}

		private static void UpdateModuleStoreSchema(ModuleDatabase database, string dbName, ModuleInstanceCache cache) {
			database.GetModuleInstanceCache(cache.AssemblyInfo.Assembly).UpdateDatabase(database.ForceUpdateCheck);
		}
	}
}
