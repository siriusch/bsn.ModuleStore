using System;
using System.Text;

namespace bsn.ModuleStore {
	public abstract class SqlManifestResourceAttribute: SqlAssemblyAttribute {
		private readonly string manifestResourceName;

		internal SqlManifestResourceAttribute(Type type, string embeddedResourceName) {
			if (string.IsNullOrEmpty(embeddedResourceName)) {
				throw new ArgumentNullException("embeddedResourceName");
			}
			if (type != null) {
				StringBuilder builder = new StringBuilder();
				string ns = type.Namespace;
				if (!string.IsNullOrEmpty(ns)) {
					builder.Append(ns);
					builder.Append(Type.Delimiter);
				}
				builder.Append(embeddedResourceName);
				this.manifestResourceName = builder.ToString();
			} else {
				this.manifestResourceName = embeddedResourceName;
			}
		}

		public string ManifestResourceName {
			get {
				return manifestResourceName;
			}
		}
	}
}