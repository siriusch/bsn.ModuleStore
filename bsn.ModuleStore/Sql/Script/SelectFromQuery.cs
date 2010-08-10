using System;
using System.Collections.Generic;
using System.IO;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SelectFromQuery: SelectQuery {
		private readonly FromClause fromClause;
		private readonly List<Expression> groupByClause;
		private readonly Predicate havingClause;
		private readonly List<OrderExpression> orderList;
		private readonly Predicate whereClause;

		[Rule("<SelectQuery> ::= SELECT <Restriction> <TopLegacy> <ColumnItemList> <IntoClause> <FromClause> <WhereClause> <GroupClause> <HavingClause> <OptionalOrderClause> <UnionClause>", ConstructorParameterMapping = new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10})]
		public SelectFromQuery(TopExpression top, DuplicateRestrictionToken restriction, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, FromClause fromClause, Optional<Predicate> whereClause, Optional<Sequence<Expression>> groupByClause, Optional<Predicate> havingClause,
		                       Optional<Sequence<OrderExpression>> orderList, UnionClause unionClause): base(top, restriction, columnItems, intoClause, unionClause) {
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

		public List<Expression> GroupByClause {
			get {
				return groupByClause;
			}
		}

		public Predicate HavingClause {
			get {
				return havingClause;
			}
		}

		public List<OrderExpression> OrderList {
			get {
				return orderList;
			}
		}

		public Predicate WhereClause {
			get {
				return whereClause;
			}
		}

		protected override void WriteToInternal(TextWriter writer) {
			writer.WriteScript(fromClause, Environment.NewLine, null);
			writer.WriteScript(whereClause, Environment.NewLine, null);
			if (groupByClause.Count > 0) {
				writer.WriteLine();
				writer.Write("GROUP BY ");
				writer.WriteSequence(groupByClause, null, ", ", null);
			}
			writer.WriteScript(havingClause, Environment.NewLine, null);
			if (orderList.Count > 0) {
				writer.WriteLine();
				writer.Write("ORDER BY ");
				writer.WriteSequence(orderList, null, ", ", null);
			}
		}
	}
}