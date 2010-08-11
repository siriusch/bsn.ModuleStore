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

		public override void WriteTo(TextWriter writer) {
			writer.Write("CREATE ");
			if (unique) {
				writer.Write("UNIQUE ");
			}
			writer.WriteValue(clustered, null, " ");
			writer.Write("INDEX ");
			writer.WriteScript(IndexName);
			writer.Write(" ON ");
			writer.WriteScript(TableName);
			writer.Write(" (");
			writer.WriteSequence(indexColumns, null, ", ", null);
			writer.Write(") INCLUDE (");
			writer.WriteSequence(columnNames, null, ", ", null);
			writer.Write(')');
			writer.WriteIndexOptions(IndexOptions);
		}
	}
}