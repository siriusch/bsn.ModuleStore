using System;

namespace bsn.ModuleStore {
	[AttributeUsage(AttributeTargets.Assembly|AttributeTargets.Interface|AttributeTargets.Class|AttributeTargets.Method, AllowMultiple=true, Inherited=true)]
	public sealed class SqlSetupScriptAttribute: SqlSetupScriptAttributeBase {
		public SqlSetupScriptAttribute(Type type, string embeddedResourceName): base(type, embeddedResourceName) {}

		public SqlSetupScriptAttribute(string embeddedResourceName): this(null, embeddedResourceName) {}
	}
}