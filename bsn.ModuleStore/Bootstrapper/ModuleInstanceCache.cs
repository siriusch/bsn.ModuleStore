using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace bsn.ModuleStore.Bootstrapper {
	internal class ModuleInstanceCache {
		private static readonly Regex rxPrefixExtractor = new Regex(@"[^#@\.\s]+$", RegexOptions.CultureInvariant|RegexOptions.RightToLeft);
		private readonly ModuleAssemblyInfo assemblyInfo;

		private readonly SortedList<string, ModuleInstance> instances = new SortedList<string, ModuleInstance>(StringComparer.OrdinalIgnoreCase);
		private readonly ModuleDatabase owner;
		private bool dirty;

		public ModuleInstanceCache(ModuleDatabase owner, Assembly assembly) {
			if (owner == null) {
				throw new ArgumentNullException("owner");
			}
			if (assembly == null) {
				throw new ArgumentNullException("assembly");
			}
			this.owner = owner;
			assemblyInfo = ModuleAssemblyInfo.Get(assembly);
			dirty = true;
		}

		public ModuleAssemblyInfo AssemblyInfo {
			get {
				return assemblyInfo;
			}
		}

		public bool Dirty {
			get {
				return dirty;
			}
		}

		public ModuleDatabase Owner {
			get {
				return owner;
			}
		}

		public string CreateInstance() {
			return CreateInstanceInternal().Module.Schema;
		}

		public ModuleInstance GetInstance(string instance) {
			return GetInstanceInternal(instance, false);
		}

		public ModuleInstance GetDefaultInstance(bool autoCreate) {
			return GetInstanceInternal(null, autoCreate);
		}

		private ModuleInstance GetInstanceInternal(string instance, bool autoCreateDefaultInstance) {
			lock (instances) {
				LoadModules(false);
				if (string.IsNullOrEmpty(instance)) {
					using (IEnumerator<ModuleInstance> enumerator = instances.Values.GetEnumerator()) {
						if (enumerator.MoveNext()) {
							ModuleInstance firstMatch = enumerator.Current;
							if (!enumerator.MoveNext()) {
								return firstMatch;
							}
						} else if (autoCreateDefaultInstance) {
							return CreateInstanceInternal();
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

		public ICollection<string> ListInstanceNames() {
			lock (instances) {
				LoadModules(false);
				return instances.Keys.ToArray();
			}
		}

		public IEnumerable<Module> ListInstances() {
			lock (instances) {
				LoadModules(false);
				return instances.Values.Select(instance => instance.Module);
			}
		}

		public void SetDirty() {
			dirty = true;
		}

		public void UpdateDatabase(bool force) {
			lock (instances) {
				bool commit = false;
				owner.BeginSmoTransaction();
				try {
					LoadModules(false);
					foreach (ModuleInstance instance in instances.Values) {
						if (force || (assemblyInfo.Inventory.UpdateVersion > instance.Module.UpdateVersion) || (!HashWriter.HashEqual(instance.Module.SetupHash, assemblyInfo.Inventory.GetInventoryHash()))) {
							owner.UpdateInstanceDatabaseSchema(assemblyInfo.Inventory, instance.Module);
						}
					}
					LoadModules(true);
					commit = true;
				} finally {
					owner.EndSmoTransaction(commit);
				}
			}
		}

		private ModuleInstance CreateInstanceInternal() {
			lock (instances) {
				bool commit = false;
				owner.BeginSmoTransaction();
				try {
					Module module = owner.ModuleStore.Add(null, assemblyInfo.AssemblyGuid, rxPrefixExtractor.Match(assemblyInfo.Assembly.GetName().Name).Value, assemblyInfo.Assembly.FullName);
					Debug.Assert(!module.SchemaExists);
					string moduleSchema = module.Schema;
					owner.CreateInstanceDatabaseSchema(assemblyInfo.Inventory, moduleSchema);
					owner.ModuleStore.Update(module.Id, assemblyInfo.Assembly.FullName, assemblyInfo.Inventory.GetInventoryHash(), assemblyInfo.Inventory.UpdateVersion);
					ModuleInstance instance = new ModuleInstance(this, module);
					instances.Add(moduleSchema, instance);
					commit = true;
					return instance;
				} finally {
					owner.EndSmoTransaction(commit);
				}
			}
		}

		private void LoadModules(bool force) {
			if (dirty || force) {
				lock (instances) {
					dirty = false;
					try {
						HashSet<string> toRemove = new HashSet<string>(instances.Keys);
						lock (owner.ModuleStore) {
							foreach (Module module in owner.ModuleStore.List(assemblyInfo.AssemblyGuid)) {
								toRemove.Remove(module.Schema);
								ModuleInstance instance;
								if (instances.TryGetValue(module.Schema, out instance)) {
									instance.Module = module;
								} else {
									instances.Add(module.Schema, new ModuleInstance(this, module));
								}
							}
						}
						foreach (string schemaName in toRemove) {
							instances.Remove(schemaName);
						}
					} catch {
						dirty = true;
						throw;
					}
				}
			}
		}
	}
}