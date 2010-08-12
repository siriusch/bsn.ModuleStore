using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateColumnIndexStatement: CreateIndexStatement {
		private readonly Clustered clustered;
		private readonly List<ColumnName> includeColumnNames;
		private readonly List<IndexColumn> indexColumns;
		private readonly bool unique;

		[Rule("<CreateIndexStatement> ::= CREATE <IndexOptionalUnique> <ConstraintCluster> INDEX <IndexName> ON <TableName> '(' <IndexColumnList> ')' <IndexOptionGroup>", ConstructorParameterMapping = new[] {1, 2, 4, 6, 8, 10})]
		public CreateColumnIndexStatement(Optional<UniqueToken> unique, ConstraintClusterToken clustered, IndexName indexName, TableName tableName, Sequence<IndexColumn> indexColumns, Optional<Sequence<IndexOption>> indexOptions)
				: this(unique, clustered, indexName, tableName, indexColumns, null, indexOptions) {}

		[Rule("<CreateIndexStatement> ::= CREATE <IndexOptionalUnique> <ConstraintCluster> INDEX <IndexName> ON <TableName> '(' <IndexColumnList> ')' INCLUDE_ <ColumnNameList> ')' <IndexOptionGroup>", ConstructorParameterMapping = new[] {1, 2, 4, 6, 8, 11, 13})]
		public CreateColumnIndexStatement(Optional<UniqueToken> unique, ConstraintClusterToken clustered, IndexName indexName, TableName tableName, Sequence<IndexColumn> indexColumns, Sequence<ColumnName> columnNames, Optional<Sequence<IndexOption>> indexOptions): base(indexName, tableName, indexOptions) {
			Debug.Assert(clustered != null);
			this.unique = unique.HasValue();
			this.clustered = clustered.Clustered;
			this.indexColumns = indexColumns.ToList();
			includeColumnNames = columnNames.ToList();
		}

		public Clustered Clustered {
			get {
				return clustered;
			}
		}

		public List<ColumnName> IncludeColumnNames {
			get {
				return includeColumnNames;
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
			writer.IncreaseIndent();
			writer.WriteSequence(indexColumns, WhitespacePadding.NewlineBefore, ", ");
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write(')');
			if (includeColumnNames.Count > 0) {
				writer.Write(" INCLUDE (");
				writer.IncreaseIndent();
				writer.WriteSequence(includeColumnNames, WhitespacePadding.NewlineBefore, ", ");
				writer.DecreaseIndent();
				writer.WriteLine();
				writer.Write(')');
			}
			writer.WriteIndexOptions(IndexOptions);
		}
	}
}