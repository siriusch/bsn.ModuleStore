using System;

namespace bsn.ModuleStore {
	public sealed class SqlSetupScriptAttribute: SqlManifestResourceAttribute {
		public SqlSetupScriptAttribute(Type type, string embeddedResourceName): base(type, embeddedResourceName) {}

		public SqlSetupScriptAttribute(string embeddedResourceName): this(null, embeddedResourceName) {}
	}
}