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
//  
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

namespace bsn.ModuleStore {
	public class ModuleDatabase: IDisposable {
		public static DatabaseType GetDatabaseType(string connectionString) {
			string dummy;
			return Bootstrap.GetDatabaseType(connectionString, out dummy);
		}

		[Conditional("DEBUG")]
		private static void DebugWriteFirstLines(string sql) {
			StringBuilder result = new StringBuilder();
			bool hasMore;
			using (StringReader reader = new StringReader(sql)) {
				for (int i = 0; i < 3; i++) {
					string line = reader.ReadLine();
					if (line == null) {
						break;
					}
					line = line.Trim();
					if (line.Length > 0) {
						result.Append(line);
						result.Append(' ');
					}
				}
				hasMore = (reader.ReadLine() != null);
			}
			if (result.Length > 0) {
				if (hasMore) {
					result.Append("...");
				}
				Debug.WriteLine(result, "Executing SQL");
			}
		}

		private readonly bool autoUpdate;
		private readonly string connectionString;
		private readonly Dictionary<Assembly, ModuleInstanceCache> instances = new Dictionary<Assembly, ModuleInstanceCache>();
		private readonly IModules moduleStore;
		private readonly ManagementConnectionProvider managementConnectionProvider;
		private bool forceUpdateCheck = Debugger.IsAttached;

		public ModuleDatabase(string connectionString): this(connectionString, false) {}

		public ModuleDatabase(string connectionString, bool autoUpdate) {
			Debug.WriteLine(DateTime.Now, "Start DB initialization");
			this.connectionString = connectionString;
			this.autoUpdate = autoUpdate;
			managementConnectionProvider = new ManagementConnectionProvider(connectionString, "ModuleStore");
			moduleStore = SqlCallProxy.Create<IModules>(managementConnectionProvider);
			Bootstrap.InitializeModuleStore(this);
		}

		public bool AutoUpdate {
			get {
				return autoUpdate;
			}
		}

		public bool ForceUpdateCheck {
			get {
				return forceUpdateCheck;
			}
			set {
				forceUpdateCheck = value;
			}
		}

		protected internal string ConnectionString {
			get {
				return connectionString;
			}
		}

		protected internal ManagementConnectionProvider ManagementConnectionProvider {
			get {
				return managementConnectionProvider;
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
			return Get<TI>(false);
		}

		public TI Get<TI>(bool autoCreate) where TI: IStoredProcedures {
			return GetModuleInstanceCache(typeof(TI).Assembly).GetDefaultInstance(autoCreate).GetProxy<TI>();
		}

		public TI Get<TI>(string instance) where TI: IStoredProcedures {
			return GetModuleInstanceCache(typeof(TI).Assembly).GetInstance(instance).GetProxy<TI>();
		}

		public AssemblyInventory GetInventory(Assembly assembly) {
			return GetModuleInstanceCache(assembly).AssemblyInfo.Inventory;
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

		protected internal virtual IConnectionProvider CreateConnectionProvider(string schema) {
			return new ConnectionProvider(ConnectionString, schema);
		}

		protected internal virtual void CreateInstanceDatabaseSchema(AssemblyInventory inventory, string moduleSchema) {
			if (inventory == null) {
				throw new ArgumentNullException("inventory");
			}
			if (string.IsNullOrEmpty(moduleSchema)) {
				throw new ArgumentNullException("moduleSchema");
			}
			AssertSmoTransaction();
			foreach (string sql in inventory.GenerateInstallSql(moduleSchema)) {
				using (SqlCommand command = managementConnectionProvider.GetConnection().CreateCommand()) {
					command.CommandType = CommandType.Text;
					command.CommandText = sql;
					command.ExecuteNonQuery();
				}
			}
		}

		protected internal virtual void UpdateInstanceDatabaseSchema(AssemblyInventory inventory, Module module) {
			if (inventory == null) {
				throw new ArgumentNullException("inventory");
			}
			if (module == null) {
				throw new ArgumentNullException("module");
			}
			AssertSmoTransaction();
			DatabaseInventory databaseInventory = new DatabaseInventory(managementConnectionProvider, module.Schema);
			bool hasChanges = !HashWriter.HashEqual(databaseInventory.GetInventoryHash(), inventory.GetInventoryHash());
			foreach (string sql in inventory.GenerateUpdateSql(databaseInventory, module.UpdateVersion)) {
				DebugWriteFirstLines(sql);
				hasChanges = true;
				using (SqlCommand command = managementConnectionProvider.GetConnection().CreateCommand()) {
					command.CommandType = CommandType.Text;
					command.CommandText = sql;
					command.ExecuteNonQuery();
				}
			}
			if (hasChanges) {
				databaseInventory = new DatabaseInventory(managementConnectionProvider, module.Schema);
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

		protected void AssertSmoTransaction() {
			if (managementConnectionProvider.GetTransaction() == null) {
				throw new InvalidOperationException("Management operations must be in a transaction");
			}
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (managementConnectionProvider != null) {
					managementConnectionProvider.Dispose();
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
					if (autoUpdate && (assembly != Assembly.GetExecutingAssembly())) {
						moduleInstances.UpdateDatabase(ForceUpdateCheck);
					}
					instances.Add(assembly, moduleInstances);
				}
			}
			return moduleInstances;
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			Dispose(true);
		}
	}
}
