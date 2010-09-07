using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace bsn.ModuleStore.Sql {
	public interface IAssemblyHandle {
		AssemblyName AssemblyName {
			get;
		}

		KeyValuePair<T, string>[] GetCustomAttributes<T>() where T: Attribute;
		string[] GetManifestResourceNames();
		Stream GetManifestResourceStream(Type type, string streamName);
	}
}