using System;
using System.Collections.Generic;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class SelectFromQuery: SelectQuery {
		private readonly FromClause fromClause;
		private readonly Predicate whereClause;
		private readonly List<Expression> groupByClause;
		private readonly Predicate havingClause;
		private readonly List<OrderExpression> orderList;

		[Rule("<SelectQuery> ::= SELECT <Restriction> <TopLegacy> <ColumnItemList> <IntoClause> <FromClause> <WhereClause> <GroupClause> <HavingClause> <OptionalOrderClause> <UnionClause>", ConstructorParameterMapping = new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10})]
		public SelectFromQuery(TopExpression top, DuplicateRestrictionToken restriction, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, FromClause fromClause, Optional<Predicate> whereClause, Optional<Sequence<Expression>> groupByClause, Optional<Predicate> havingClause,
		                       Optional<Sequence<OrderExpression>> orderList, UnionClause unionClause): base(top, restriction, columnItems, intoClause, unionClause) {
			this.fromClause = fromClause;
			this.whereClause = whereClause;
			this.groupByClause = groupByClause.ToList();
			this.havingClause = havingClause;
			this.orderList = orderList.ToList();
		}

		protected override void WriteToInternal(System.IO.TextWriter writer) {
			
		}
	}
}