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
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateColumnIndexStatement: CreateIndexStatement {
		private readonly Clustered clustered;
		private readonly Predicate filter;
		private readonly List<ColumnName> includeColumnNames;
		private readonly List<IndexColumn> indexColumns;
		private readonly bool unique;

		[Rule("<CreateIndexStatement> ::= ~CREATE <IndexOptionalUnique> <ConstraintCluster> ~INDEX <IndexName> ~ON <TableNameQualified> ~'(' <IndexColumnList> ~')' <IndexOptionGroup>")]
		public CreateColumnIndexStatement(Optional<UniqueToken> unique, ConstraintClusterToken clustered, IndexName indexName, Qualified<SchemaName, TableName> tableName, Sequence<IndexColumn> indexColumns, Optional<Sequence<IndexOption>> indexOptions)
				: this(unique, clustered, indexName, tableName, indexColumns, null, null, indexOptions) {}

		[Rule("<CreateIndexStatement> ::= ~CREATE <IndexOptionalUnique> <ConstraintCluster> ~INDEX <IndexName> ~ON <TableNameQualified> ~'(' <IndexColumnList> ~')' ~WHERE <Predicate> <IndexOptionGroup>")]
		public CreateColumnIndexStatement(Optional<UniqueToken> unique, ConstraintClusterToken clustered, IndexName indexName, Qualified<SchemaName, TableName> tableName, Sequence<IndexColumn> indexColumns, Predicate filter, Optional<Sequence<IndexOption>> indexOptions)
				: this(unique, clustered, indexName, tableName, indexColumns, null, filter, indexOptions) {}

		[Rule("<CreateIndexStatement> ::= ~CREATE <IndexOptionalUnique> <ConstraintCluster> ~INDEX <IndexName> ~ON <TableNameQualified> ~'(' <IndexColumnList> ~')' ~INCLUDE ~'(' <ColumnNameList> ~')' <IndexOptionGroup>")]
		public CreateColumnIndexStatement(Optional<UniqueToken> unique, ConstraintClusterToken clustered, IndexName indexName, Qualified<SchemaName, TableName> tableName, Sequence<IndexColumn> indexColumns, Sequence<ColumnName> columnNames, Optional<Sequence<IndexOption>> indexOptions)
				: this(unique, clustered, indexName, tableName, indexColumns, columnNames, null, indexOptions) {}

		[Rule("<CreateIndexStatement> ::= ~CREATE <IndexOptionalUnique> <ConstraintCluster> ~INDEX <IndexName> ~ON <TableNameQualified> ~'(' <IndexColumnList> ~')' ~INCLUDE ~'(' <ColumnNameList> ~')' ~WHERE <Predicate> <IndexOptionGroup>")]
		public CreateColumnIndexStatement(Optional<UniqueToken> unique, ConstraintClusterToken clustered, IndexName indexName, Qualified<SchemaName, TableName> tableName, Sequence<IndexColumn> indexColumns, Sequence<ColumnName> columnNames, Predicate filter, Optional<Sequence<IndexOption>> indexOptions)
				: base(indexName, tableName, indexOptions) {
			Debug.Assert(clustered != null);
			this.unique = unique.HasValue();
			this.clustered = clustered.Clustered;
			this.indexColumns = indexColumns.ToList();
			this.filter = filter;
			includeColumnNames = columnNames.ToList();
		}

		public Clustered Clustered {
			get {
				return clustered;
			}
		}

		public Predicate Filter {
			get {
				return filter;
			}
		}

		public IEnumerable<ColumnName> IncludeColumnNames {
			get {
				return includeColumnNames;
			}
		}

		public IEnumerable<IndexColumn> IndexColumns {
			get {
				return indexColumns;
			}
		}

		public bool Unique {
			get {
				return unique;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("CREATE ");
			if (unique) {
				writer.Write("UNIQUE ");
			}
			writer.WriteEnum(clustered, WhitespacePadding.SpaceAfter);
			writer.Write("INDEX ");
			writer.WriteScript(IndexName, WhitespacePadding.None);
			writer.Write(" ON ");
			writer.WriteScript(TableName, WhitespacePadding.None);
			writer.Write(" (");
			writer.IncreaseIndent();
			writer.WriteScriptSequence(indexColumns, WhitespacePadding.NewlineBefore, ", ");
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write(')');
			if (includeColumnNames.Count > 0) {
				writer.Write(" INCLUDE (");
				writer.IncreaseIndent();
				writer.WriteScriptSequence(includeColumnNames, WhitespacePadding.NewlineBefore, ", ");
				writer.DecreaseIndent();
				writer.WriteLine();
				writer.Write(')');
			}
			if ((filter != null) && writer.IsAtLeast(DatabaseEngine.SqlServer2008)) {
				writer.WriteLine();
				writer.IncreaseIndent();
				writer.WriteScript(filter, WhitespacePadding.None, "WHERE ", "");
				writer.DecreaseIndent();
				if (IndexOptions.Any()) {
					writer.WriteLine();
				}
			}
			writer.WriteIndexOptions(IndexOptions);
		}
	}
}
