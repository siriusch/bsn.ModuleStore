using System;
using System.IO;
using System.Reflection;

namespace bsn.ModuleStore.Console {
	public class AssemblyHandle: MarshalByRefObject {
		private readonly Assembly assembly;

		public AssemblyHandle(string assemblyFileName) {
			if (assemblyFileName == null) {
				throw new ArgumentNullException("assemblyFileName");
			}
			this.assembly = Assembly.ReflectionOnlyLoadFrom(assemblyFileName);
		}

		public T[] GetCustomAttributes<T>() where T: Attribute {
			return (T[])assembly.GetCustomAttributes(typeof(T), false);
		}

		public Stream GetManifestResourceStream(string streamName) {
			return assembly.GetManifestResourceStream(streamName);
		}

		public string[] GetManifestResourceNames() {
			return assembly.GetManifestResourceNames();
		}

		public AssemblyName AssemblyName {
			get {
				return assembly.GetName();
			}
		}
	}
}