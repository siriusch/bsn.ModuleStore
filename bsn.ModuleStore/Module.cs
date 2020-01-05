﻿// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;

using bsn.ModuleStore.Bootstrapper;
using bsn.ModuleStore.Mapper;
using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore {
	public class Module {
		// ReSharper disable FieldCanBeMadeReadOnly.Local
#pragma warning disable 649
		[SqlColumn("uidAssemblyGuid")]
		private Guid assemblyGuid;

		[SqlColumn("uidModule", Identity = true)]
		private Guid id;

		[SqlColumn("sSchema")]
		private string schema;

		[SqlColumn("fSchemaExists")]
		private bool schemaExists;

		[SqlColumn("dtSetup", DateTimeKind = DateTimeKind.Utc)]
		private DateTime setupDate;

		[SqlColumn("binSetupHash")]
		private byte[] setupHash;

		[SqlColumn("dtUpdate", DateTimeKind = DateTimeKind.Utc)]
		private DateTime? updateDate;

		[SqlColumn("iUpdateVersion")]
		private int updateVersion;
#pragma warning restore 649
		// ReSharper restore FieldCanBeMadeReadOnly.Local

		private ModuleInstanceCache owner;

		public Guid AssemblyGuid => assemblyGuid;

		public ModuleDatabase Database => owner.Owner;

		public Guid Id => id;

		public string Schema => schema;

		public bool SchemaExists => schemaExists;

		public DateTime SetupDate => setupDate;

		public byte[] SetupHash => setupHash;

		public DateTime UpdateDate => updateDate ?? setupDate;

		public int UpdateVersion => updateVersion;

		public DatabaseInventory GetInventory() {
			var database = owner.Owner;
			database.ManagementConnectionProvider.BeginTransaction();
			try {
				return new DatabaseInventory(database.ManagementConnectionProvider, schema);
			} finally {
				database.ManagementConnectionProvider.EndTransaction(false);
			}
		}

		internal void SetOwner(ModuleInstanceCache owner) {
			if (owner == null) {
				throw new ArgumentNullException(nameof(owner));
			}
			this.owner = owner;
			Debug.Assert(owner.AssemblyInfo.AssemblyGuid == assemblyGuid);
		}
	}
}
