using System;
using System.Collections.Generic;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SelectFromQuery: SelectQuery {
		private readonly FromClause fromClause;
		private readonly List<Expression> groupByClause;
		private readonly Predicate havingClause;
		private readonly List<OrderExpression> orderList;
		private readonly Predicate whereClause;

		[Rule("<SelectQuery> ::= ~SELECT <Restriction> <TopLegacy> <ColumnItemList> <IntoClause> <FromClause> <WhereClause> <GroupClause> <HavingClause> <OptionalOrderClause> <RowsetCombineClause>")]
		public SelectFromQuery(DuplicateRestrictionToken restriction, TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, FromClause fromClause, Optional<Predicate> whereClause, Optional<Sequence<Expression>> groupByClause, Optional<Predicate> havingClause,
		                       Optional<Sequence<OrderExpression>> orderList, RowsetCombineClause unionClause): this(restriction.Distinct, top, columnItems, intoClause, fromClause, whereClause, groupByClause, havingClause, orderList, null, unionClause) {}

		[Rule("<SelectQuery> ::= ~SELECT <Restriction> <ColumnItemList> <IntoClause> <FromClause> <WhereClause> <GroupClause> <HavingClause> <OptionalOrderClause> <RowsetCombineClause>")]
		public SelectFromQuery(DuplicateRestrictionToken restriction, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, FromClause fromClause, Optional<Predicate> whereClause, Optional<Sequence<Expression>> groupByClause, Optional<Predicate> havingClause,
		                       Optional<Sequence<OrderExpression>> orderList, RowsetCombineClause unionClause): this(restriction.Distinct, null, columnItems, intoClause, fromClause, whereClause, groupByClause, havingClause, orderList, null, unionClause) {}

		[Rule("<SelectQuery> ::= ~SELECT <TopLegacy> <ColumnItemList> <IntoClause> <FromClause> <WhereClause> <GroupClause> <HavingClause> <OptionalOrderClause> <RowsetCombineClause>")]
		public SelectFromQuery(TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, FromClause fromClause, Optional<Predicate> whereClause, Optional<Sequence<Expression>> groupByClause, Optional<Predicate> havingClause, Optional<Sequence<OrderExpression>> orderList,
		                       RowsetCombineClause unionClause): this(default(bool?), top, columnItems, intoClause, fromClause, whereClause, groupByClause, havingClause, orderList, null, unionClause) {}

		[Rule("<SelectQuery> ::= ~SELECT <ColumnItemList> <IntoClause> <FromClause> <WhereClause> <GroupClause> <HavingClause> <OptionalOrderClause> <RowsetCombineClause>")]
		public SelectFromQuery(Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, FromClause fromClause, Optional<Predicate> whereClause, Optional<Sequence<Expression>> groupByClause, Optional<Predicate> havingClause, Optional<Sequence<OrderExpression>> orderList,
		                       RowsetCombineClause unionClause): this(default(bool?), null, columnItems, intoClause, fromClause, whereClause, groupByClause, havingClause, orderList, null, unionClause) {}

		[Rule("<SelectQuery> ::= ~SELECT <Restriction> <TopLegacy> <ColumnItemList> <IntoClause> <FromClause> <WhereClause> <GroupClause> <HavingClause> <OptionalOrderClause> <ForClause>")]
		public SelectFromQuery(DuplicateRestrictionToken restriction, TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, FromClause fromClause, Optional<Predicate> whereClause, Optional<Sequence<Expression>> groupByClause, Optional<Predicate> havingClause,
		                       Optional<Sequence<OrderExpression>> orderList, ForClause forClause): this(restriction.Distinct, top, columnItems, intoClause, fromClause, whereClause, groupByClause, havingClause, orderList, forClause, null) {}

		[Rule("<SelectQuery> ::= ~SELECT <Restriction> <ColumnItemList> <IntoClause> <FromClause> <WhereClause> <GroupClause> <HavingClause> <OptionalOrderClause> <ForClause>")]
		public SelectFromQuery(DuplicateRestrictionToken restriction, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, FromClause fromClause, Optional<Predicate> whereClause, Optional<Sequence<Expression>> groupByClause, Optional<Predicate> havingClause,
		                       Optional<Sequence<OrderExpression>> orderList, ForClause forClause): this(restriction.Distinct, null, columnItems, intoClause, fromClause, whereClause, groupByClause, havingClause, orderList, forClause, null) {}

		[Rule("<SelectQuery> ::= ~SELECT <TopLegacy> <ColumnItemList> <IntoClause> <FromClause> <WhereClause> <GroupClause> <HavingClause> <OptionalOrderClause> <ForClause>")]
		public SelectFromQuery(TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, FromClause fromClause, Optional<Predicate> whereClause, Optional<Sequence<Expression>> groupByClause, Optional<Predicate> havingClause, Optional<Sequence<OrderExpression>> orderList,
		                       ForClause forClause): this(default(bool?), top, columnItems, intoClause, fromClause, whereClause, groupByClause, havingClause, orderList, forClause, null) {}

		[Rule("<SelectQuery> ::= ~SELECT <ColumnItemList> <IntoClause> <FromClause> <WhereClause> <GroupClause> <HavingClause> <OptionalOrderClause> <ForClause>")]
		public SelectFromQuery(Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, FromClause fromClause, Optional<Predicate> whereClause, Optional<Sequence<Expression>> groupByClause, Optional<Predicate> havingClause, Optional<Sequence<OrderExpression>> orderList,
		                       ForClause forClause): this(default(bool?), null, columnItems, intoClause, fromClause, whereClause, groupByClause, havingClause, orderList, forClause, null) {}

		private SelectFromQuery(bool? restriction, TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, FromClause fromClause, Optional<Predicate> whereClause, Optional<Sequence<Expression>> groupByClause, Optional<Predicate> havingClause,
		                        Optional<Sequence<OrderExpression>> orderList, ForClause forClause, RowsetCombineClause unionClause): base(restriction, top, columnItems, intoClause, forClause, unionClause) {
			this.fromClause = fromClause;
			this.whereClause = whereClause;
			this.groupByClause = groupByClause.ToList();
			this.havingClause = havingClause;
			this.orderList = orderList.ToList();
		}

		public FromClause FromClause {
			get {
				return fromClause;
			}
		}

		public IEnumerable<Expression> GroupByClause {
			get {
				return groupByClause;
			}
		}

		public Predicate HavingClause {
			get {
				return havingClause;
			}
		}

		public IEnumerable<OrderExpression> OrderList {
			get {
				return orderList;
			}
		}

		public Predicate WhereClause {
			get {
				return whereClause;
			}
		}

		protected override void WriteToInternal(SqlWriter writer) {
			writer.WriteScript(fromClause, WhitespacePadding.NewlineBefore);
			writer.WriteScript(whereClause, WhitespacePadding.NewlineBefore, "WHERE ", null);
			if (groupByClause.Count > 0) {
				writer.WriteLine();
				writer.Write("GROUP BY ");
				writer.WriteScriptSequence(groupByClause, WhitespacePadding.None, ", ");
			}
			writer.WriteScript(havingClause, WhitespacePadding.NewlineBefore);
			if (orderList.Count > 0) {
				writer.WriteLine();
				writer.Write("ORDER BY ");
				writer.WriteScriptSequence(orderList, WhitespacePadding.None, ", ");
			}
		}
	}
}