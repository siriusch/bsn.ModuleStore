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

using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateFulltextIndexStatement: AlterableCreateStatement {
		private readonly FulltextChangeTracking changeTracking;
		private readonly List<FulltextColumn> columns;
		private readonly Qualified<SchemaName, TableName> tableName;
		private IndexName indexName;

		[Rule("<CreateFulltextStatement> ::= ~CREATE ~FULLTEXT ~INDEX ~ON ~TABLE <TableNameQualified> <FulltextColumnGroup> ~KEY ~INDEX <IndexName> <FulltextChangeTracking>")]
		public CreateFulltextIndexStatement(Qualified<SchemaName, TableName> tableName, Optional<Sequence<FulltextColumn>> columns, IndexName indexName, FulltextChangeTracking changeTracking) {
			Debug.Assert(tableName != null);
			Debug.Assert(indexName != null);
			Debug.Assert(changeTracking != null);
			this.tableName = tableName;
			this.columns = columns.ToList();
			this.indexName = indexName;
			this.changeTracking = changeTracking;
		}

		public FulltextChangeTracking ChangeTracking {
			get {
				return changeTracking;
			}
		}

		public IEnumerable<FulltextColumn> Columns {
			get {
				return columns;
			}
		}

		public IndexName IndexName {
			get {
				return indexName;
			}
		}

		public override ObjectCategory ObjectCategory {
			get {
				return ObjectCategory.Index;
			}
		}

		public override string ObjectName {
			get {
				return indexName.Value;
			}
			set {
				indexName = new IndexName(value);
			}
		}

		public Qualified<SchemaName, TableName> TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("CREATE FULLTEXT INDEX ON TABLE ");
			writer.WriteScript(tableName, WhitespacePadding.None);
			if (columns.Count > 0) {
				writer.Write(" (");
				writer.WriteScriptSequence(columns, WhitespacePadding.None, ", ");
				writer.Write(')');
			}
			writer.Write(" KEY INDEX ");
			writer.WriteScript(indexName, WhitespacePadding.None);
			writer.WriteScript(changeTracking, WhitespacePadding.SpaceBefore);
		}

		protected override IInstallStatement CreateDropStatement() {
			return new DropFulltextStatement(tableName);
		}

		protected override string GetObjectSchema() {
			return tableName.IsQualified ? tableName.Qualification.Value : string.Empty;
		}
	}
}
