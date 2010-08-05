using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class SelectQuery: SqlToken {
		[Rule("<SelectQuery> ::= SELECT <TopLegacy> <Restriction> <ColumnItemList> <IntoClause> <UnionClause>", ConstructorParameterMapping = new[] {1, 2, 3, 4, 5})]
		public SelectQuery(TopExpression top, DuplicateRestriction restriction, Sequence<ColumnItem> columnItems, Optional<SqlName> intoClause, UnionClause unionClause) {}
	}
}