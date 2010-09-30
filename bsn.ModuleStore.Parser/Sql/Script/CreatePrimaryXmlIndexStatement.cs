using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreatePrimaryXmlIndexStatement: CreateXmlIndexStatement {
		[Rule("<CreateIndexStatement> ::= ~CREATE ~PRIMARY ~XML ~INDEX <IndexName> ~ON <TableNameQualified> ~'(' <ColumnName> ~')' <IndexUsing> <IndexOptionGroup>")]
		public CreatePrimaryXmlIndexStatement(IndexName indexName, Qualified<SchemaName, TableName> tableName, ColumnName columnName, IndexUsing indexUsing, Optional<Sequence<IndexOption>> indexOptions): base(indexName, tableName, columnName, indexUsing, indexOptions) {}

		public override bool Primary {
			get {
				return true;
			}
		}
	}
}