using System;

namespace bsn.ModuleStore {
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true, Inherited=true)]
	public abstract class SqlAssemblyAttribute: Attribute {
		internal SqlAssemblyAttribute() {}
	}
}