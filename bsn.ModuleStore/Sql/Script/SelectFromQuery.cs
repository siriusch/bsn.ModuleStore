using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class SelectFromQuery: SelectQuery {
		[Rule("<SelectQuery> ::= SELECT <TopLegacy> <Restriction> <ColumnItemList> <IntoClause> <FromClause> <WhereClause> <GroupClause> <HavingClause> <OptionalOrderClause> <UnionClause>", ConstructorParameterMapping = new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10})]
		public SelectFromQuery(TopExpression top, DuplicateRestriction restriction, Sequence<ColumnItem> columnItems, Optional<SqlName> intoClause, FromClause fromClause, Optional<Predicate> whereClause, Optional<Sequence<Expression>> groupByClause, Optional<Predicate> havingClause,
		                       Optional<Sequence<Order>> orderList, UnionClause unionClause): base(top, restriction, columnItems, intoClause, unionClause) {}
	}
}