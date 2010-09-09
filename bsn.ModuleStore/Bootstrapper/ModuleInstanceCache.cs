using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;

using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore.Bootstrapper {
	internal class ModuleInstanceCache {
		private static readonly Regex rxPrefixExtractor = new Regex(@"[^#@\.\s]+$", RegexOptions.CultureInvariant|RegexOptions.RightToLeft);

		private readonly Assembly assembly;
		private readonly Guid assemblyGuid;
		private readonly SortedList<string, ModuleInstance> instances = new SortedList<string, ModuleInstance>(StringComparer.OrdinalIgnoreCase);
		private readonly AssemblyInventory inventory;
		private readonly Database owner;
		private bool dirty;

		public ModuleInstanceCache(Database owner, Assembly assembly) {
			if (owner == null) {
				throw new ArgumentNullException("owner");
			}
			if (assembly == null) {
				throw new ArgumentNullException("assembly");
			}
			this.owner = owner;
			this.assembly = assembly;
			inventory = new AssemblyInventory(assembly);
			using (IEnumerator<GuidAttribute> guidAttributeEnumerator = assembly.GetCustomAttributes(typeof(GuidAttribute), true).Cast<GuidAttribute>().GetEnumerator()) {
				if (guidAttributeEnumerator.MoveNext()) {
					GuidAttribute guidAttribute = guidAttributeEnumerator.Current;
					Debug.Assert(guidAttribute != null);
					assemblyGuid = new Guid(guidAttribute.Value);
				} else {
					Debug.WriteLine(string.Format("Inferring GUID from assembly short name for assembly {0}", assembly.FullName));
					using (MD5 md5 = MD5.Create()) {
						assemblyGuid = new Guid(md5.ComputeHash(Encoding.Unicode.GetBytes(assembly.GetName().Name)));
					}
				}
			}
			dirty = true;
		}

		public Assembly Assembly {
			get {
				return assembly;
			}
		}

		public Guid AssemblyGuid {
			get {
				return assemblyGuid;
			}
		}

		public bool Dirty {
			get {
				return dirty;
			}
		}

		public AssemblyInventory Inventory {
			get {
				return inventory;
			}
		}

		public Database Owner {
			get {
				return owner;
			}
		}

		public string CreateInstance() {
			lock (instances) {
				using (TransactionScope scope = new TransactionScope()) {
					Module module = owner.ModuleStore.Add(null, assemblyGuid, rxPrefixExtractor.Match(assembly.GetName().Name).Value, assembly.FullName);
					Debug.Assert(!module.SchemaExists);
					string moduleSchema = module.Schema;
					owner.CreateInstanceDatabaseSchema(inventory, moduleSchema);
					owner.ModuleStore.Update(module.Id, assembly.FullName, inventory.GetInventoryHash(), inventory.SetupUpdateVersion);
					instances.Add(moduleSchema, new ModuleInstance(this, module));
					scope.Complete();
					return moduleSchema;
				}
			}
		}

		public ModuleInstance GetInstance(string instance) {
			lock (instances) {
				LoadIfDirty();
				if (string.IsNullOrEmpty(instance)) {
					if (instances.Count == 1) {
						foreach (ModuleInstance moduleInstance in instances.Values) {
							return moduleInstance;
						}
					}
					throw new InvalidOperationException(string.Format("There is no single (default) instance available ({0} instances)", instances.Count));
				}
				ModuleInstance result;
				if (instances.TryGetValue(instance, out result)) {
					return result;
				}
				throw new KeyNotFoundException(instance);
			}
		}

		public IEnumerable<string> ListInstances() {
			lock (instances) {
				LoadIfDirty();
				return instances.Keys.ToArray();
			}
		}

		public void SetDirty() {
			dirty = true;
		}

		public void UpdateDatabase(bool force) {
			lock (instances) {
				LoadIfDirty();
				foreach (ModuleInstance instance in instances.Values) {
					if (force || (!HashWriter.HashEqual(instance.Module.SetupHash, inventory.GetInventoryHash()))) {
						owner.UpdateInstanceDatabaseSchema(inventory, instance.Module.Schema);
					}
				}
			}
		}

		private void LoadIfDirty() {
			HashSet<string> toRemove = new HashSet<string>(instances.Keys);
			foreach (Module module in owner.ModuleStore.List(assemblyGuid)) {
				toRemove.Remove(module.Schema);
				ModuleInstance instance;
				if (instances.TryGetValue(module.Schema, out instance)) {
					instance.Module = module;
				} else {
					instances.Add(module.Schema, new ModuleInstance(this, module));
				}
			}
			foreach (string schemaName in toRemove) {
				instances.Remove(schemaName);
			}
		}
	}
}