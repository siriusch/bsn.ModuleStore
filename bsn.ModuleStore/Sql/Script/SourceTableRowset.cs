using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class SourceTableRowset: SourceRowset {
		[Rule("<SourceRowset> ::= <TableNameQualified> <OptionalAlias>")]
		public SourceTableRowset(Qualified<TableName> tableName, Optional<AliasName> aliasName): base(aliasName) {}
	}
}