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