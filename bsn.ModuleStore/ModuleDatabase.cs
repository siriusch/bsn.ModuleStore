using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

using bsn.ModuleStore.Bootstrapper;
using bsn.ModuleStore.Mapper;
using bsn.ModuleStore.Sql;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace bsn.ModuleStore {
	public sealed class ModuleDatabase {
		public static DatabaseType GetDatabaseType(string connectionString) {
			string dummy;
			return Bootstrap.GetDatabaseType(connectionString, out dummy);
		}

		private readonly bool autoUpdate;
		private readonly string connectionString;
		private readonly Dictionary<Assembly, ModuleInstanceCache> instances = new Dictionary<Assembly, ModuleInstanceCache>();
		private readonly IModules moduleStore;

		public ModuleDatabase(string connectionString): this(connectionString, false) {}

		public ModuleDatabase(string connectionString, bool autoUpdate) {
			Debug.WriteLine(DateTime.Now, "Start DB initialization");
			this.connectionString = connectionString;
			this.autoUpdate = autoUpdate;
			Bootstrap.InitializeModuleStore(this, out moduleStore);
		}

		public bool AutoUpdate {
			get {
				return autoUpdate;
			}
		}

		internal string ConnectionString {
			get {
				return connectionString;
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

		public ICollection<string> ListInstanceNames(Assembly assembly) {
			return GetModuleInstanceCache(assembly).ListInstanceNames();
		}

		public IEnumerable<Module> ListInstances(Assembly assembly) {
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
				connection.Open();
				foreach (string sql in inventory.GenerateInstallSql(moduleSchema)) {
					using (SqlCommand command = connection.CreateCommand()) {
						command.CommandType = CommandType.Text;
						command.CommandText = sql;
						command.ExecuteNonQuery();
					}
				}
			}
		}

		internal ModuleInstanceCache GetModuleInstanceCache(Assembly assembly) {
			if (assembly == null) {
				throw new ArgumentNullException("assembly");
			}
			ModuleInstanceCache moduleInstances;
			lock (instances) {
				if (!instances.TryGetValue(assembly, out moduleInstances)) {
					moduleInstances = new ModuleInstanceCache(this, assembly);
					if (autoUpdate) {
						moduleInstances.UpdateDatabase(false);
					}
					instances.Add(assembly, moduleInstances);
				}
			}
			return moduleInstances;
		}

		internal void UpdateInstanceDatabaseSchema(AssemblyInventory inventory, Module module) {
			using (SqlConnection connection = CreateConnection()) {
				connection.Open();
				Server server = new Server(new ServerConnection(connection));
				Database database = server.Databases[connection.Database];
				DatabaseInventory databaseInventory = new DatabaseInventory(database, module.Schema);
				bool hasChanges = !HashWriter.HashEqual(databaseInventory.GetInventoryHash(), inventory.GetInventoryHash());
				foreach (string sql in inventory.GenerateUpdateSql(databaseInventory, module.UpdateVersion)) {
					hasChanges = true;
					using (SqlCommand command = connection.CreateCommand()) {
						command.CommandType = CommandType.Text;
						command.CommandText = sql;
						command.ExecuteNonQuery();
					}
				}
				if (hasChanges) {
					// check fails because SMO somehow does not see the DDL performed in the scope of the transaction
					/*
					databaseInventory = new DatabaseInventory(database, module.Schema);
					string[] differences = Inventory.Compare(inventory, databaseInventory).Where(pair => pair.Value != InventoryObjectDifference.None).Select(pair => pair.Key.ObjectName).ToArray();
					if (differences.Length > 0) {
						throw new InvalidOperationException("Update failed for: "+string.Join(", ", differences));
					}
					 */
				}
			}
			moduleStore.Update(module.Id, inventory.AssemblyName.FullName, inventory.GetInventoryHash(), inventory.UpdateVersion);
		}
	}
}