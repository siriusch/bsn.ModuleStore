using System;

namespace bsn.ModuleStore {
	public sealed class SqlUpdateScriptAttribute: SqlManifestResourceAttribute {
		private readonly int version;

		public SqlUpdateScriptAttribute(int version, Type type, string embeddedResourceName): base(type, embeddedResourceName) {
			if (version < 1) {
				throw new ArgumentOutOfRangeException("version");
			}
			this.version = version;
		}

		public SqlUpdateScriptAttribute(int version, string embeddedResourceName): this(version, null, embeddedResourceName) {}

		public int Version {
			get {
				return version;
			}
		}
	}
}