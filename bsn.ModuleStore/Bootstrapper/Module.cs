using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.ModuleStore.Mapper;

namespace bsn.ModuleStore.Bootstrapper {
	public class Module {
		[SqlColumn("uidModule")]
		private Guid id;

		[SqlColumn("uidAssemblyGuid")]
		private Guid assemblyGuid;

		[SqlColumn("sSchema")]
		private string schema;

		[SqlColumn("binSetupHash")]
		private byte[] setupHash;

		[SqlColumn("iUpdateVersion")]
		private int updateVersion;

		[SqlColumn("dtSetup")]
		private DateTime setupDate;

		[SqlColumn("dtUpdate")]
		private DateTime updateDate;

		[SqlColumn("fSchemaExists")]
		private bool schemaExists;

		public Guid Id {
			get {
				return id;
			}
		}

		public Guid AssemblyGuid {
			get {
				return assemblyGuid;
			}
		}

		public string Schema {
			get {
				return schema;
			}
		}

		public byte[] SetupHash {
			get {
				return setupHash;
			}
		}

		public int UpdateVersion {
			get {
				return updateVersion;
			}
		}

		public DateTime SetupDate {
			get {
				return setupDate;
			}
		}

		public DateTime UpdateDate {
			get {
				return updateDate;
			}
		}

		public bool SchemaExists {
			get {
				return schemaExists;
			}
		}
	}
}
