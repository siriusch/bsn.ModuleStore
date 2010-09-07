using System;

namespace bsn.ModuleStore {
	public class SqlSetupScriptAttributeBase: SqlManifestResourceAttribute {
		internal SqlSetupScriptAttributeBase(Type type, string embeddedResourceName): base(type, embeddedResourceName) {}
	}
}