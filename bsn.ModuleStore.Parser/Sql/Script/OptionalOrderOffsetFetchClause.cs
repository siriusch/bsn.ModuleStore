// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2019 by Arsène von Wyss - avw@gmx.ch
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

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script
{
	public class OptionalOrderOffsetFetchClause: SqlToken, IOptional
	{
		private readonly Sequence<OrderExpression> orderList;
		private readonly Expression offsetExpression;
		private readonly Expression fetchExpression;

		[Rule("<OptionalOrderOffsetFetchClause> ::= <OrderClause> ~OFFSET <Expression> ~ROWS ~FETCH ~NEXT <Expression> ~ROWS ~ONLY")]
		[Rule("<OptionalOrderOffsetFetchClause> ::= <OrderClause> ~OFFSET <Expression> ~ROWS ~FETCH ~FIRST <Expression> ~ROWS ~ONLY")]
		public OptionalOrderOffsetFetchClause(Sequence<OrderExpression> orderList, Expression offsetExpression, Expression fetchExpression) {
			this.orderList = orderList;
			this.offsetExpression = offsetExpression;
			this.fetchExpression = fetchExpression;
		}

		[Rule("<OptionalOrderOffsetFetchClause> ::= <OrderClause> ~OFFSET <Expression> ~ROWS")]
		public OptionalOrderOffsetFetchClause(Sequence<OrderExpression> orderList, Expression offsetExpression): this(orderList, offsetExpression, null) {}

		[Rule("<OptionalOrderOffsetFetchClause> ::= <OrderClause>")]
		public OptionalOrderOffsetFetchClause(Sequence<OrderExpression> orderList): this(orderList, null, null) {}

		[Rule("<OptionalOrderOffsetFetchClause> ::=")]
		public OptionalOrderOffsetFetchClause(): this(null, null, null) {}

		public Sequence<OrderExpression> OrderList => orderList;

		public Expression OffsetExpression => offsetExpression;

		public Expression FetchExpression => fetchExpression;

		public bool HasValue => orderList != null;
	}
}
