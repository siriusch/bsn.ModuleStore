using System;
using System.Text;

namespace bsn.ModuleStore {
	public abstract class SqlManifestResourceAttribute: SqlAssemblyAttribute {
		private readonly string manifestResourceName;
		private readonly Type type;

		internal SqlManifestResourceAttribute(Type type, string embeddedResourceName) {
			if (string.IsNullOrEmpty(embeddedResourceName)) {
				throw new ArgumentNullException("embeddedResourceName");
			}
			this.type = type;
			this.manifestResourceName = embeddedResourceName;
		}

		public Type ManifestResourceType {
			get {
				return type;
			}
		}

		public string ManifestResourceName {
			get {
				return manifestResourceName;
			}
		}
	}
}