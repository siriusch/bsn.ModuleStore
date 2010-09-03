using System;
using System.IO;
using System.Reflection;

namespace bsn.ModuleStore.Sql {
	public interface IAssemblyHandle: ICustomAttributeProvider {
		AssemblyName AssemblyName {
			get;
		}

		string[] GetManifestResourceNames();
		Stream GetManifestResourceStream(string streamName);
	}
}