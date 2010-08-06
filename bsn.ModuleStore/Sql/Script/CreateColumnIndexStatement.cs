using System;
using System.Collections.Generic;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateColumnIndexStatement: CreateIndexStatement {
		private readonly Clustered clustered;
		private readonly List<ColumnName> columnNames;
		private readonly List<IndexColumn> indexColumns;
		private readonly List<IndexOption> indexOptions;
		private readonly bool unique;

		[Rule("<CreateIndexStatement> ::= CREATE <IndexOptionalUnique> <ConstraintCluster> INDEX <IndexName> ON <TableName> '(' <IndexColumnList> ')' INCLUDE_ <ColumnNameList> ')' <IndexOptionGroup>", ConstructorParameterMapping = new[] {1, 2, 4, 6, 8, 11, 13})]
		public CreateColumnIndexStatement(Optional<UniqueToken> unique, ConstraintClusterToken clustered, IndexName indexName, TableName tableName, Sequence<IndexColumn> indexColumns, Sequence<ColumnName> columnNames, Optional<Sequence<IndexOption>> indexOptions): base(indexName, tableName, indexOptions) {
			if (clustered == null) {
				throw new ArgumentNullException("clustered");
			}
			this.unique = unique.HasValue();
			this.clustered = clustered.Clustered;
			this.indexColumns = indexColumns.ToList();
			this.columnNames = columnNames.ToList();
			this.indexOptions = indexOptions.ToList();
		}
	}
}