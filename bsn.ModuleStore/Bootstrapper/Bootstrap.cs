﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Transactions;

using bsn.ModuleStore.Mapper;

namespace bsn.ModuleStore.Bootstrapper {
	internal static class Bootstrap {
		internal static DatabaseType GetDatabaseType(string connectionString, out string dbName) {
			using (SqlConnection connection = new SqlConnection(connectionString)) {
				dbName = connection.Database;
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
					command.CommandText = "SELECT COUNT(*) FROM [INFORMATION_SCHEMA].[SCHEMATA] WHERE [SCHEMA_NAME]='ModuleStore'";
					command.CommandType = CommandType.Text;
					if (Convert.ToBoolean(command.ExecuteScalar())) {
						return DatabaseType.ModuleStore;
					}
				}
				using (SqlCommand command = connection.CreateCommand()) {
					command.CommandText = "SELECT COUNT(*) FROM [INFORMATION_SCHEMA].[TABLES] WHERE [TABLE_NAME] <> 'sysdiagrams'";
					command.CommandType = CommandType.Text;
					if (Convert.ToInt32(command.ExecuteScalar()) == 0) {
						return DatabaseType.Empty;
					}
				}
				return DatabaseType.Other;
			}
		}

		public static IModules InitializeModuleStore(Database database) {
			IModules result;
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew)) {
				result = SqlCallProxy.Create<IModules>(database.CreateConnection, "ModuleStore");
				Debug.WriteLine(DateTime.Now, "Got ModuleStore proxy");
				string dbName;
				ModuleInstanceCache cache = database.GetModuleInstanceCache(typeof(IModules).Assembly);
				switch (GetDatabaseType(database.ConnectionString, out dbName)) {
				case DatabaseType.Other:
					throw new InvalidOperationException(string.Format("The database {0} is not a ModuleStore database", dbName));
				case DatabaseType.Empty:
					Debug.WriteLine(DateTime.Now, "Create ModuleStore schema start");
					CreateModuleStoreSchema(database, dbName, cache);
					Debug.WriteLine(DateTime.Now, "Create ModuleStore schema end");
					break;
				case DatabaseType.ModuleStore:
					Debug.WriteLine(DateTime.Now, "Update ModuleStore schema start");
					UpdateModuleStoreSchema(database, dbName, cache);
					Debug.WriteLine(DateTime.Now, "Update ModuleStore schema end");
					break;
				case DatabaseType.None:
					throw new InvalidOperationException(string.Format("The database {0} does not exist", dbName));
				}
				scope.Complete();
			}
			Debug.WriteLine(DateTime.Now, "End DB initialization");
			return result;
		}

		private static void CreateModuleStoreSchema(Database database, string dbName, ModuleInstanceCache cache) {
			database.CreateInstanceDatabaseSchema(cache.Inventory, "ModuleStore");
			using (SqlConnection connection = database.CreateConnection()) {
				connection.Open();
				using (SqlCommand command = connection.CreateCommand()) {
					command.CommandType = CommandType.Text;
					command.CommandText = "INSERT [ModuleStore].[tblModule] ([uidAssemblyGuid], [sSchema], [sAssemblyName], [binSetupHash], [iUpdateVersion]) VALUES (@uidAssemblyGuid, 'ModuleStore', @sAssemblyName, @binSetupHash, @iUpdateVersion)";
					command.Parameters.AddWithValue("@uidAssemblyGuid", cache.AssemblyGuid);
					command.Parameters.AddWithValue("@sAssemblyName", cache.Assembly.FullName);
					command.Parameters.AddWithValue("@binSetupHash", cache.Inventory.GetInventoryHash());
					command.Parameters.AddWithValue("@iUpdateVersion", cache.Inventory.UpdateVersion);
					command.ExecuteNonQuery();
				}
			}
			Trace.WriteLine(string.Format("Installed ModuleStore in database {0}", dbName));
		}

		private static void UpdateModuleStoreSchema(Database database, string dbName, ModuleInstanceCache cache) {
			database.GetModuleInstanceCache(cache.Assembly).UpdateDatabase(false);
		}
	}
}