using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace bsn.ModuleStore.Console {
	public class AssemblyReflectionLoader {
		public AssemblyReflectionLoader() {
			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ResolveAssembly;
		}

		private Assembly ResolveAssembly(object sender, ResolveEventArgs args) {
			try {
				return Assembly.ReflectionOnlyLoad(args.Name);
			} catch (Exception ex) {
				Debug.WriteLine(ex);
				return null;
			}
		}
	}
}
