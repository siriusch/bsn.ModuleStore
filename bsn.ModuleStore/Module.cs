using System;
using System.Diagnostics;

using bsn.ModuleStore.Bootstrapper;
using bsn.ModuleStore.Mapper;
using bsn.ModuleStore.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace bsn.ModuleStore {
	public class Module {
		[SqlColumn("uidAssemblyGuid")]
		private Guid assemblyGuid;

		[SqlColumn("uidModule")]
		private Guid id;

		private ModuleInstanceCache owner;

		[SqlColumn("sSchema")]
		private string schema;

		[SqlColumn("fSchemaExists")]
		private bool schemaExists;

		[SqlColumn("dtSetup", DateTimeKind = DateTimeKind.Utc)]
		private DateTime setupDate;

		[SqlColumn("binSetupHash")]
		private byte[] setupHash;

		[SqlColumn("dtUpdate", DateTimeKind = DateTimeKind.Utc)]
		private DateTime updateDate;

		[SqlColumn("iUpdateVersion")]
		private int updateVersion;

		public Guid AssemblyGuid {
			get {
				return assemblyGuid;
			}
		}

		public ModuleDatabase Database {
			get {
				return owner.Owner;
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

		public DatabaseInventory GetInventory() {
			ModuleDatabase database = owner.Owner;
			database.BeginSmoTransaction();
			try {
				Server server = new Server(database.SmoConnectionProvider.ServerConnection);
				return new DatabaseInventory(server.Databases[database.SmoConnectionProvider.DatabaseName], schema);
			} finally {
				database.EndSmoTransaction(false);
			}
		}

		internal void SetOwner(ModuleInstanceCache owner) {
			if (owner == null) {
				throw new ArgumentNullException("owner");
			}
			this.owner = owner;
			Debug.Assert(owner.AssemblyInfo.AssemblyGuid == assemblyGuid);
		}
	}
}