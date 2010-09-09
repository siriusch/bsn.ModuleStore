using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Transactions;

using bsn.ModuleStore.Mapper;
using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore.Bootstrapper {
	public sealed class Database {
		public static DatabaseType GetDatabaseType(string connectionString) {
			string dummy;
			return GetDatabaseType(connectionString, out dummy);
		}

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

		private readonly string connectionString;
		private readonly Dictionary<Assembly, ModuleInstanceCache> instances = new Dictionary<Assembly, ModuleInstanceCache>();
		private readonly IModules moduleStore;

		public Database(string connectionString) {
			this.connectionString = connectionString;
			using (TransactionScope scope = new TransactionScope()) {
				moduleStore = SqlCallProxy.Create<IModules>(CreateConnection, "ModuleStore");
				string dbName;
				ModuleInstanceCache cache = GetModuleInstanceCache(typeof(IModules).Assembly);
				switch (GetDatabaseType(connectionString, out dbName)) {
				case DatabaseType.Other:
					throw new InvalidOperationException(string.Format("The database {0} is not a ModuleStore database", dbName));
				case DatabaseType.Empty:
					CreateInstanceDatabaseSchema(cache.Inventory, "ModuleStore");
					using (SqlConnection connection = CreateConnection()) {
						using (SqlCommand command = connection.CreateCommand()) {
							command.CommandType = CommandType.Text;
							command.CommandText = "INSERT [ModuleStore].[tblModule] ([uidAssemblyGuid], [sSchema], [sAssemblyName], [binSetupHash], [iUpdateVersion]) VALUES (@uidAssemblyGuid, 'ModuleStore', @sAssemblyName, @binSetupHash, @iUpdateVersion)";
							command.Parameters.AddWithValue("@uidAssemblyGuid", cache.AssemblyGuid);
							command.Parameters.AddWithValue("@sAssemblyName", cache.Assembly.FullName);
							command.Parameters.AddWithValue("@binSetupHash", cache.Inventory.GetInventoryHash());
							command.Parameters.AddWithValue("@iUpdateVersion", cache.Inventory.SetupUpdateVersion);
							command.ExecuteNonQuery();
						}
					}
					Trace.WriteLine("Installed ModuleStore in empty database");
					break;
				case DatabaseType.ModuleStore:
					GetModuleInstanceCache(cache.Assembly).UpdateDatabase(false);
					break;
				case DatabaseType.None:
					throw new InvalidOperationException(string.Format("The database {0} does not exist", dbName));
				}
				scope.Complete();
			}
		}

		internal IModules ModuleStore {
			get {
				return moduleStore;
			}
		}

		public string CreateInstance(Assembly assembly) {
			return GetModuleInstanceCache(assembly).CreateInstance();
		}

		public TI Get<TI>() where TI: IStoredProcedures {
			return Get<TI>(null);
		}

		public TI Get<TI>(string instance) where TI: IStoredProcedures {
			return GetModuleInstanceCache(typeof(TI).Assembly).GetInstance(instance).GetProxy<TI>();
		}

		public IEnumerable<string> ListInstances(Assembly assembly) {
			return GetModuleInstanceCache(assembly).ListInstances();
		}

		public void Refresh() {
			lock (instances) {
				foreach (ModuleInstanceCache cache in instances.Values) {
					cache.SetDirty();
				}
			}
		}

		internal SqlConnection CreateConnection() {
			return new SqlConnection(connectionString);
		}

		internal void CreateInstanceDatabaseSchema(AssemblyInventory inventory, string moduleSchema) {
			if (string.IsNullOrEmpty(moduleSchema)) {
				throw new ArgumentNullException("moduleSchema");
			}
			using (SqlConnection connection = CreateConnection()) {
				foreach (string sql in inventory.GenerateInstallSql(moduleSchema)) {
					using (SqlCommand command = connection.CreateCommand()) {
						command.CommandType = CommandType.Text;
						command.CommandText = sql;
						command.ExecuteNonQuery();
					}
				}
			}
		}

		internal void UpdateInstanceDatabaseSchema(AssemblyInventory inventory, string moduleSchema) {
			throw new NotImplementedException();
		}

		private ModuleInstanceCache GetModuleInstanceCache(Assembly assembly) {
			if (assembly == null) {
				throw new ArgumentNullException("assembly");
			}
			ModuleInstanceCache moduleInstances;
			lock (instances) {
				if (!instances.TryGetValue(assembly, out moduleInstances)) {
					moduleInstances = new ModuleInstanceCache(this, assembly);
					instances.Add(assembly, moduleInstances);
				}
			}
			return moduleInstances;
		}
	}
}