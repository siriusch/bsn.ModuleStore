using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateColumnIndexStatement: CreateIndexStatement {
		private readonly Clustered clustered;
		private readonly List<ColumnName> columnNames;
		private readonly List<IndexColumn> indexColumns;
		private readonly bool unique;

		[Rule("<CreateIndexStatement> ::= CREATE <IndexOptionalUnique> <ConstraintCluster> INDEX <IndexName> ON <TableName> '(' <IndexColumnList> ')' INCLUDE_ <ColumnNameList> ')' <IndexOptionGroup>", ConstructorParameterMapping = new[] {1, 2, 4, 6, 8, 11, 13})]
		public CreateColumnIndexStatement(Optional<UniqueToken> unique, ConstraintClusterToken clustered, IndexName indexName, TableName tableName, Sequence<IndexColumn> indexColumns, Sequence<ColumnName> columnNames, Optional<Sequence<IndexOption>> indexOptions): base(indexName, tableName, indexOptions) {
			Debug.Assert(clustered != null);
			this.unique = unique.HasValue();
			this.clustered = clustered.Clustered;
			this.indexColumns = indexColumns.ToList();
			this.columnNames = columnNames.ToList();
		}

		public Clustered Clustered {
			get {
				return clustered;
			}
		}

		public List<ColumnName> ColumnNames {
			get {
				return columnNames;
			}
		}

		public List<IndexColumn> IndexColumns {
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
			writer.WriteSequence(indexColumns, WhitespacePadding.None, ", ");
			writer.Write(") INCLUDE (");
			writer.WriteSequence(columnNames, WhitespacePadding.None, ", ");
			writer.Write(')');
			writer.WriteIndexOptions(IndexOptions);
		}
	}
}