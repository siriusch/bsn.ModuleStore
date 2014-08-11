// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

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
		                        Optional<Sequence<OrderExpression>> orderList, ForClause forClause, RowsetCombineClause unionClause): base(restriction, top, columnItems, intoClause, whereClause, forClause, unionClause) {
			this.fromClause = fromClause;
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

		protected override void WriteToInternal(SqlWriter writer) {
			writer.WriteScript(fromClause, WhitespacePadding.NewlineBefore);
			base.WriteToInternal(writer);
			if (groupByClause.Count > 0) {
				writer.WriteLine();
				writer.WriteKeyword("GROUP BY ");
				writer.WriteScriptSequence(groupByClause, WhitespacePadding.None, w => w.Write(", "));
			}
			writer.WriteScript(havingClause, WhitespacePadding.NewlineBefore, w => w.WriteKeyword("HAVING "), null);
			if (orderList.Count > 0) {
				writer.WriteLine();
				writer.WriteKeyword("ORDER BY ");
				writer.WriteScriptSequence(orderList, WhitespacePadding.None, w => w.Write(", "));
			}
		}
	}
}
