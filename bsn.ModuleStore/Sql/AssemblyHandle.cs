using System;
using System.IO;
using System.Reflection;

namespace bsn.ModuleStore.Sql {
	public class AssemblyHandle: MarshalByRefObject, ICustomAttributeProvider {
		private readonly Assembly assembly;

		public AssemblyHandle(string assemblyFile): this(Assembly.ReflectionOnlyLoadFrom(assemblyFile)) {}

		public AssemblyHandle(Assembly assembly) {
			if (assembly == null) {
				throw new ArgumentNullException("assembly");
			}
			this.assembly = assembly;
		}

		public AssemblyName AssemblyName {
			get {
				return assembly.GetName();
			}
		}

		public string[] GetManifestResourceNames() {
			return assembly.GetManifestResourceNames();
		}

		public Stream GetManifestResourceStream(string streamName) {
			return assembly.GetManifestResourceStream(streamName);
		}

		public object[] GetCustomAttributes(Type attributeType, bool inherit) {
			return assembly.GetCustomAttributes(attributeType, inherit);
		}

		public object[] GetCustomAttributes(bool inherit) {
			return assembly.GetCustomAttributes(inherit);
		}

		public bool IsDefined(Type attributeType, bool inherit) {
			return assembly.IsDefined(attributeType, inherit);
		}
	}
}