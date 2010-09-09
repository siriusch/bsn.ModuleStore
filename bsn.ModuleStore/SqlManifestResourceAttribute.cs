using System;

namespace bsn.ModuleStore {
	public abstract class SqlManifestResourceAttribute: SqlAssemblyAttribute {
		private readonly string manifestResourceName;
		private readonly Type type;

		internal SqlManifestResourceAttribute(Type type, string embeddedResourceName) {
			if (string.IsNullOrEmpty(embeddedResourceName)) {
				throw new ArgumentNullException("embeddedResourceName");
			}
			this.type = type;
			manifestResourceName = embeddedResourceName;
		}

		public string ManifestResourceName {
			get {
				return manifestResourceName;
			}
		}

		public Type ManifestResourceType {
			get {
				return type;
			}
		}
	}
}