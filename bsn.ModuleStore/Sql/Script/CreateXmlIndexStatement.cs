using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateXmlIndexStatement: CreateIndexStatement {
		[Rule("<CreateIndexStatement> ::= CREATE <IndexPrimary> XML_INDEX <IndexName> ON <TableName> '(' <ColumnName> ')' <IndexUsing> <IndexOptionGroup>", ConstructorParameterMapping = new[] {1, 3, 5, 7, 9, 10})]
		public CreateXmlIndexStatement(Optional<Primary> primary, IndexName indexName, TableName tableName, ColumnName columnName, IndexUsing indexUsing, Optional<Sequence<IndexOption>> indexOptions): base(indexName, tableName, indexOptions) {}
	}
}