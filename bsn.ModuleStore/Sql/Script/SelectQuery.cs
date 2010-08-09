using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class SelectQuery: SqlToken, IScriptable {
		[Rule("<SelectQuery> ::= SELECT <TopLegacy> <Restriction> <ColumnItemList> <IntoClause> <UnionClause>", ConstructorParameterMapping = new[] {1, 2, 3, 4, 5})]
		public SelectQuery(TopExpression top, DuplicateRestrictionToken restriction, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, UnionClause unionClause) {}
	}
}