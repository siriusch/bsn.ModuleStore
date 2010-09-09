using System;
using System.Collections.Generic;

using bsn.ModuleStore.Mapper;

namespace bsn.ModuleStore.Bootstrapper {
	internal class ModuleInstance {
		private readonly ModuleInstanceCache owner;
		private readonly Dictionary<Type, IStoredProcedures> proxies = new Dictionary<Type, IStoredProcedures>();
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
			}
		}

		public TI GetProxy<TI>() where TI: IStoredProcedures {
			lock (proxies) {
				IStoredProcedures proxy;
				if (proxies.TryGetValue(typeof(TI), out proxy)) {
					return (TI)proxy;
				}
				TI result = SqlCallProxy.Create<TI>(owner.Owner.CreateConnection, module.Schema);
				proxies.Add(typeof(TI), result);
				return result;
			}
		}
	}
}