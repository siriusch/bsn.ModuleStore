using System;
using System.Collections.Generic;

using bsn.ModuleStore.Mapper;

namespace bsn.ModuleStore.Bootstrapper {
	internal class ModuleInstance {
		private readonly ModuleInstanceCache owner;
		private readonly Dictionary<Type, IStoredProcedures> proxies = new Dictionary<Type, IStoredProcedures>();
		private bool isUpToDate;
		private Module module;

		public ModuleInstance(ModuleInstanceCache owner, Module module) {
			if (owner == null) {
				throw new ArgumentNullException("owner");
			}
			if (module == null) {
				throw new ArgumentNullException("module");
			}
			this.owner = owner;
			this.module = module;
			UpdateUpToDateStatus();
		}

		public bool IsUpToDate {
			get {
				return isUpToDate;
			}
		}

		public Module Module {
			get {
				return module;
			}
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				module = value;
				UpdateUpToDateStatus();
			}
		}

		public void AssertIsUpToDate() {
			if (!IsUpToDate) {
				throw new InvalidOperationException(string.Format("The module instance {0} is not up to date", module.Schema));
			}
		}

		public TI GetProxy<TI>() where TI: IStoredProcedures {
			lock (proxies) {
				IStoredProcedures proxy;
				if (proxies.TryGetValue(typeof(TI), out proxy)) {
					return (TI)proxy;
				}
				AssertIsUpToDate();
				TI result = SqlCallProxy.Create<TI>(new ConnectionProvider(owner.Owner.ConnectionString, module.Schema));
				proxies.Add(typeof(TI), result);
				return result;
			}
		}

		private void UpdateUpToDateStatus() {
			isUpToDate = HashWriter.HashEqual(owner.AssemblyInfo.Inventory.GetInventoryHash(), module.SetupHash);
		}
	}
}