using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateXmlIndexStatement: CreateIndexStatement {
		[Rule("<CreateIndexStatement> ::= CREATE <IndexPrimary> XML_INDEX <IndexName> ON <TableName> '(' <ColumnName> ')' <IndexUsing> <IndexOptionGroup>", ConstructorParameterMapping = new[] {1, 3, 5, 7, 9, 10})]
		public CreateXmlIndexStatement(Optional<PrimaryToken> primary, IndexName indexName, TableName tableName, ColumnName columnName, IndexUsing indexUsing, Optional<Sequence<IndexOption>> indexOptions): base(indexName, tableName, indexOptions) {}
	}
}