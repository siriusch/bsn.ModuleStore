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
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateXmlIndexStatement: CreateIndexStatement {
		private readonly ColumnName columnName;
		private readonly IndexUsing indexUsing;

		[Rule("<CreateIndexStatement> ::= ~CREATE ~XML ~INDEX <IndexName> ~ON <TableNameQualified> ~'(' <ColumnName> ~')' <IndexUsing> <IndexOptionGroup>")]
		public CreateXmlIndexStatement(IndexName indexName, Qualified<SchemaName, TableName> tableName, ColumnName columnName, IndexUsing indexUsing, IndexOptionGroup indexOptions)
			: base(indexName, tableName, indexOptions) {
			this.columnName = columnName;
			this.indexUsing = indexUsing;
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}

		public IndexUsing IndexUsing {
			get {
				return indexUsing;
			}
		}

		public virtual bool Primary {
			get {
				return false;
			}
		}

		public override bool DoesApplyToEngine(DatabaseEngine engine) {
			if (engine == DatabaseEngine.SqlAzure) {
				return false;
			}
			return base.DoesApplyToEngine(engine);
		}

		public override void WriteTo(SqlWriter writer) {
			Debug.Assert(writer.Engine != DatabaseEngine.SqlAzure);
			WriteCommentsTo(writer);
			writer.WriteKeyword("CREATE ");
			if (Primary) {
				writer.WriteKeyword("PRIMARY ");
			}
			writer.WriteKeyword("XML INDEX ");
			writer.WriteScript(IndexName, WhitespacePadding.None);
			writer.WriteKeyword(" ON ");
			writer.WriteScript(TableName, WhitespacePadding.None);
			writer.Write(" (");
			using (writer.Indent()) {
				writer.WriteScript(columnName, WhitespacePadding.NewlineBefore);
			}
			writer.WriteLine();
			writer.Write(") ");
			writer.WriteScript(indexUsing, WhitespacePadding.None);
			writer.WriteScript(IndexOptions, WhitespacePadding.NewlineBefore);
		}
	}
}
