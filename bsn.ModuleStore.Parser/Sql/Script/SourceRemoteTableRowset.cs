// bsn ModuleStore database versioning
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
//  
using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceRemoteTableRowset: SourceRowset {
		private readonly Identifier databaseName;
		private readonly SchemaName schemaName;
		private readonly Identifier serverName;
		private readonly TableName tableName;

		[Rule("<SourceRowset> ::= <SchemaName> ~'.' <TableName> ~'.' <TableName> <RowsetAlias>")]
		public SourceRemoteTableRowset(SchemaName databaseName, TableName schemaName, TableName tableName, RowsetAlias rowsetAlias): this(string.Empty, databaseName.Value, new SchemaName(schemaName.Value), tableName, rowsetAlias) {}

		[Rule("<SourceRowset> ::= <SchemaName> ~'.' <TableName> ~'.' <SchemaName> ~'.' <TableName> <RowsetAlias>")]
		public SourceRemoteTableRowset(SchemaName serverName, TableName databaseName, SchemaName schemaName, TableName tableName, RowsetAlias rowsetAlias): this(serverName.Value, databaseName.Value, schemaName, tableName, rowsetAlias) {}

		private SourceRemoteTableRowset(string serverName, string databaseName, SchemaName schemaName, TableName tableName, RowsetAlias rowsetAlias): base(rowsetAlias) {
			Debug.Assert(tableName != null);
			if (!string.IsNullOrEmpty(serverName)) {
				this.serverName = new Identifier(serverName);
			}
			this.databaseName = new Identifier(databaseName);
			this.schemaName = schemaName;
			this.tableName = tableName;
		}

		public Identifier DatabaseName {
			get {
				return databaseName;
			}
		}

		public SchemaName SchemaName {
			get {
				return schemaName;
			}
		}

		public Identifier ServerName {
			get {
				return serverName;
			}
		}

		public TableName TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(serverName, WhitespacePadding.None, null, ".");
			writer.WriteScript(databaseName, WhitespacePadding.None, null, ".");
			writer.WriteScript(schemaName, WhitespacePadding.None, null, ".");
			writer.WriteScript(tableName, WhitespacePadding.None);
			base.WriteTo(writer);
		}
	}
}
