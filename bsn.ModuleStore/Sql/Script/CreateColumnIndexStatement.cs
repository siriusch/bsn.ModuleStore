using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateColumnIndexStatement: CreateIndexStatement {
		[Rule("<CreateIndexStatement> ::= CREATE <IndexOptionalUnique> <ConstraintCluster> INDEX <IndexName> ON <TableName> '(' <IndexColumnList> ')' INCLUDE_ <ColumnNameList> ')' <IndexOptionGroup>", ConstructorParameterMapping = new[] {1, 2, 4, 6, 8, 11, 13})]
		public CreateColumnIndexStatement(Optional<Unique> unique, Clustered clustered, IndexName indexName, TableName tableName, Sequence<IndexColumn> indexColumns, Sequence<ColumnName> columnNames, Optional<Sequence<IndexOption>> indexOptions): base(indexName, tableName, indexOptions) {}
	}
}