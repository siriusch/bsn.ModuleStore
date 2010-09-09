using System;
using System.Linq;

using bsn.ModuleStore.Mapper;

namespace bsn.ModuleStore.Bootstrapper {
	public class Module {
		[SqlColumn("uidAssemblyGuid")]
		private Guid assemblyGuid;

		[SqlColumn("uidModule")]
		private Guid id;

		[SqlColumn("sSchema")]
		private string schema;

		[SqlColumn("fSchemaExists")]
		private bool schemaExists;

		[SqlColumn("dtSetup")]
		private DateTime setupDate;

		[SqlColumn("binSetupHash")]
		private byte[] setupHash;

		[SqlColumn("dtUpdate")]
		private DateTime updateDate;

		[SqlColumn("iUpdateVersion")]
		private int updateVersion;

		public Guid AssemblyGuid {
			get {
				return assemblyGuid;
			}
		}

		public Guid Id {
			get {
				return id;
			}
		}

		public string Schema {
			get {
				return schema;
			}
		}

		public bool SchemaExists {
			get {
				return schemaExists;
			}
		}

		public DateTime SetupDate {
			get {
				return setupDate;
			}
		}

		public byte[] SetupHash {
			get {
				return setupHash;
			}
		}

		public DateTime UpdateDate {
			get {
				return updateDate;
			}
		}

		public int UpdateVersion {
			get {
				return updateVersion;
			}
		}
	}
}