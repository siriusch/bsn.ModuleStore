using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

using bsn.ModuleStore.Bootstrapper;
using bsn.ModuleStore.Mapper;
using bsn.ModuleStore.Sql;
using bsn.ModuleStore.Sql.Script;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace bsn.ModuleStore {
	public sealed class ModuleDatabase: IDisposable {
		public static DatabaseType GetDatabaseType(string connectionString) {
			string dummy;
			return Bootstrap.GetDatabaseType(connectionString, out dummy);
		}

		private readonly bool autoUpdate;
		private readonly string connectionString;
		private readonly Dictionary<Assembly, ModuleInstanceCache> instances = new Dictionary<Assembly, ModuleInstanceCache>();
		private readonly IModules moduleStore;
		private readonly SmoConnectionProvider smoConnectionProvider;

		public ModuleDatabase(string connectionString): this(connectionString, false) {}

		public ModuleDatabase(string connectionString, bool autoUpdate) {
			Debug.WriteLine(DateTime.Now, "Start DB initialization");
			this.connectionString = connectionString;
			this.autoUpdate = autoUpdate;
			smoConnectionProvider = new SmoConnectionProvider(connectionString, "ModuleStore");
			moduleStore = SqlCallProxy.Create<IModules>(smoConnectionProvider);
			Bootstrap.InitializeModuleStore(this);
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

		internal void BeginSmoTransaction() {
			Monitor.Enter(moduleStore);
			try {
				smoConnectionProvider.ServerConnection.BeginTransaction();
				Debug.WriteLine(DateTime.Now, "Beginning SMO transaction");
			} catch {
				Monitor.Exit(moduleStore);
				throw;
			}
		}

		internal void EndSmoTransaction(bool commit) {
			try {
				if (commit) {
					smoConnectionProvider.ServerConnection.CommitTransaction();
					Debug.WriteLine(DateTime.Now, "Commited SMO transaction");
				} else {
					smoConnectionProvider.ServerConnection.RollBackTransaction();
					Debug.WriteLine(DateTime.Now, "Rolled back SMO transaction");
				}
			} finally {
				Monitor.Exit(moduleStore);
			}
		}

		public string CreateInstance(Assembly assembly) {
			return GetModuleInstanceCache(assembly).CreateInstance();
		}

		public TI Get<TI>() where TI: IStoredProcedures {
			return Get<TI>(false);
		}

		public TI Get<TI>(bool autoCreate) where TI: IStoredProcedures {
			return GetModuleInstanceCache(typeof(TI).Assembly).GetDefaultInstance(autoCreate).GetProxy<TI>();
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
			AssertSmoTransaction();
				foreach (string sql in inventory.GenerateInstallSql(moduleSchema)) {
					using (SqlCommand command = smoConnectionProvider.GetConnection().CreateCommand()) {
						command.CommandType = CommandType.Text;
						command.CommandText = sql;
						command.ExecuteNonQuery();
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
			AssertSmoTransaction();
			Server server = new Server(smoConnectionProvider.ServerConnection);
			Database database = server.Databases[smoConnectionProvider.DatabaseName];
			DatabaseInventory databaseInventory = new DatabaseInventory(database, module.Schema);
			bool hasChanges = !HashWriter.HashEqual(databaseInventory.GetInventoryHash(), inventory.GetInventoryHash());
			foreach (string sql in inventory.GenerateUpdateSql(databaseInventory, module.UpdateVersion)) {
				DebugWriteFirstLine(sql);
				hasChanges = true;
				using (SqlCommand command = smoConnectionProvider.GetConnection().CreateCommand()) {
					command.CommandType = CommandType.Text;
					command.CommandText = sql;
					command.ExecuteNonQuery();
				}
			}
			if (hasChanges) {
				databaseInventory = new DatabaseInventory(database, module.Schema);
				IEnumerable<KeyValuePair<CreateStatement, InventoryObjectDifference>> differences = Inventory.Compare(inventory, databaseInventory).Where(pair => pair.Value != InventoryObjectDifference.None);
				using (IEnumerator<KeyValuePair<CreateStatement, InventoryObjectDifference>> enumerator = differences.GetEnumerator()) {
					if (enumerator.MoveNext()) {
						StringBuilder message = new StringBuilder("Schema ");
						message.Append(module.Schema);
						message.Append(" update failed for");
						do {
							Trace.WriteLine(enumerator.Current.Key, string.Format("SQL object {0}.{1} is {2}", module.Schema, enumerator.Current.Key.ObjectName, enumerator.Current.Value));
							message.Append(' ');
							message.Append(enumerator.Current.Key.ObjectName);
						} while (enumerator.MoveNext());
						throw new InvalidOperationException(message.ToString());
					}
				}
			}
			moduleStore.Update(module.Id, inventory.AssemblyName.FullName, inventory.GetInventoryHash(), inventory.UpdateVersion);
		}

		private void AssertSmoTransaction() {
			if (smoConnectionProvider.ServerConnection.TransactionDepth == 0) {
				throw new InvalidOperationException("SMO operations must be in a transaction");
			}
		}

		[Conditional("DEBUG")]
		private void DebugWriteFirstLine(string sql) {
			using (StringReader reader = new StringReader(sql)) {
				string line = reader.ReadLine();
				if (!string.IsNullOrEmpty(line)) {
					Debug.WriteLine(line);
				}
			}
		}

		public void Dispose() {
			if (smoConnectionProvider != null) {
				smoConnectionProvider.Dispose();
			}
		}
	}
}