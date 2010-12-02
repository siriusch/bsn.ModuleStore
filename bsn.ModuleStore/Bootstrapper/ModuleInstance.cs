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
				TI result = SqlCallProxy.Create<TI>(owner.Owner.CreateConnectionProvider(module.Schema));
				proxies.Add(typeof(TI), result);
				return result;
			}
		}

		private void UpdateUpToDateStatus() {
			isUpToDate = HashWriter.HashEqual(owner.AssemblyInfo.Inventory.GetInventoryHash(owner.Owner.ManagementConnectionProvider.Engine), module.SetupHash);
		}
	}
}
