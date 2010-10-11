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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore.Bootstrapper {
	public sealed class ModuleAssemblyInfo {
		private static readonly Dictionary<Assembly, ModuleAssemblyInfo> cache = new Dictionary<Assembly, ModuleAssemblyInfo>();

		public static ModuleAssemblyInfo Get(Assembly assembly) {
			if (assembly == null) {
				throw new ArgumentNullException("assembly");
			}
			lock (cache) {
				ModuleAssemblyInfo result;
				if (!cache.TryGetValue(assembly, out result)) {
					result = new ModuleAssemblyInfo(assembly);
					cache.Add(assembly, result);
				}
				return result;
			}
		}

		private readonly Assembly assembly;
		private readonly Guid assemblyGuid;
		private readonly AssemblyInventory inventory;

		private ModuleAssemblyInfo(Assembly assembly) {
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

		public AssemblyInventory Inventory {
			get {
				return inventory;
			}
		}
	}
}
