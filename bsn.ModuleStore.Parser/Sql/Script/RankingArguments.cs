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

namespace bsn.ModuleStore.Sql.Script {
	public sealed class RankingArguments: SqlScriptableToken {
		private readonly List<OrderExpression> orders;
		private readonly List<Expression> partitions;

		[Rule("<RankingArguments> ::= ~PARTITION ~BY <ExpressionList> <OptionalOrderClause>")]
		public RankingArguments(Sequence<Expression> partitions, Optional<Sequence<OrderExpression>> orders): this(partitions, orders.Value) {}

		[Rule("<RankingArguments> ::= <OrderClause>")]
		public RankingArguments(Sequence<OrderExpression> orders): this(null, orders) {}

		private RankingArguments(Sequence<Expression> partitions, Sequence<OrderExpression> orders) {
			this.partitions = partitions.ToList();
			this.orders = orders.ToList();
		}

		public IEnumerable<OrderExpression> Orders {
			get {
				return orders;
			}
		}

		public IEnumerable<Expression> Partitions {
			get {
				return partitions;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			if (partitions.Count > 0) {
				writer.Write("PARTITION BY ");
				writer.WriteScriptSequence(partitions, WhitespacePadding.None, ", ");
				if (orders.Count > 0) {
					writer.Write(' ');
				}
			}
			if (orders.Count > 0) {
				writer.Write("ORDER BY ");
				writer.WriteScriptSequence(orders, WhitespacePadding.None, ", ");
			}
		}
	}
}
