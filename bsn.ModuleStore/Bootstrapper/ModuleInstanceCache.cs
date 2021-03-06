﻿// bsn ModuleStore database versioning
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
				throw new ArgumentNullException(nameof(owner));
			}
			if (assembly == null) {
				throw new ArgumentNullException(nameof(assembly));
			}
			this.owner = owner;
			assemblyInfo = ModuleAssemblyInfo.Get(assembly);
			dirty = true;
			assemblyInfo.Inventory.AssertEngineVersion(owner.ManagementConnectionProvider.EngineVersion.Major);
		}

		public ModuleAssemblyInfo AssemblyInfo => assemblyInfo;

		public bool Dirty => dirty;

		public ModuleDatabase Owner => owner;

		public string CreateInstance() {
			return CreateInstanceInternal().Module.Schema;
		}

		public ModuleInstance GetDefaultInstance(bool autoCreate) {
			return GetInstanceInternal(null, autoCreate);
		}

		public ModuleInstance GetInstance(string instance) {
			return GetInstanceInternal(instance, false);
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
				var commit = false;
				owner.ManagementConnectionProvider.BeginTransaction();
				try {
					LoadModules(false);
					foreach (var instance in instances.Values) {
						if (force || (assemblyInfo.Inventory.UpdateVersion > instance.Module.UpdateVersion) || (!HashWriter.HashEqual(instance.Module.SetupHash, assemblyInfo.Inventory.GetInventoryHash(owner.ManagementConnectionProvider.Engine)))) {
							owner.UpdateInstanceDatabaseSchema(assemblyInfo.Inventory, instance.Module);
						}
					}
					LoadModules(true);
					commit = true;
				} finally {
					owner.ManagementConnectionProvider.EndTransaction(commit);
				}
			}
		}

		private ModuleInstance CreateInstanceInternal() {
			lock (instances) {
				var commit = false;
				owner.ManagementConnectionProvider.BeginTransaction();
				try {
					var module = owner.ModuleStore.Add(null, assemblyInfo.AssemblyGuid, rxPrefixExtractor.Match(assemblyInfo.Assembly.GetName().Name).Value, assemblyInfo.Assembly.FullName);
					Debug.Assert(!module.SchemaExists);
					var moduleSchema = module.Schema;
					owner.CreateInstanceDatabaseSchema(assemblyInfo.Inventory, moduleSchema);
					owner.ModuleStore.Update(module.Id, assemblyInfo.Assembly.FullName, assemblyInfo.Inventory.GetInventoryHash(owner.ManagementConnectionProvider.Engine), assemblyInfo.Inventory.UpdateVersion);
					var instance = new ModuleInstance(this, module);
					instances.Add(moduleSchema, instance);
					LoadModules(true);
					commit = true;
					return instance;
				} finally {
					owner.ManagementConnectionProvider.EndTransaction(commit);
				}
			}
		}

		private ModuleInstance GetInstanceInternal(string instance, bool autoCreateDefaultInstance) {
			lock (instances) {
				LoadModules(false);
				if (string.IsNullOrEmpty(instance)) {
					using (var enumerator = instances.Values.GetEnumerator()) {
						if (enumerator.MoveNext()) {
							var firstMatch = enumerator.Current;
							if (!enumerator.MoveNext()) {
								return firstMatch;
							}
						} else if (autoCreateDefaultInstance) {
							return CreateInstanceInternal();
						}
					}
					throw new InvalidOperationException($"There is no single (default) instance available ({instances.Count} instances)");
				}
				if (instances.TryGetValue(instance, out var result)) {
					return result;
				}
				throw new KeyNotFoundException(instance);
			}
		}

		private void LoadModules(bool force) {
			if (dirty || force) {
				lock (instances) {
					dirty = false;
					try {
						var toRemove = new HashSet<string>(instances.Keys);
						lock (owner.ModuleStore) {
							foreach (var module in owner.ModuleStore.List(assemblyInfo.AssemblyGuid)) {
								module.SetOwner(this);
								toRemove.Remove(module.Schema);
								if (instances.TryGetValue(module.Schema, out var instance)) {
									instance.Module = module;
								} else {
									instances.Add(module.Schema, new ModuleInstance(this, module));
								}
							}
						}
						foreach (var schemaName in toRemove) {
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
