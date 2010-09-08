using System;
using System.Collections.Generic;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class RankingArguments: SqlScriptableToken {
		private readonly List<OrderExpression> orders;
		private readonly List<Expression> partitions;

		[Rule("<RankingArguments> ::= PARTITION_BY <ExpressionList> <OptionalOrderClause>", ConstructorParameterMapping = new[] {1, 2})]
		public RankingArguments(Sequence<Expression> partitions, Optional<Sequence<OrderExpression>> orders): this(partitions, orders.Value) {}

		[Rule("<RankingArguments> ::= <OrderClause>")]
		public RankingArguments(Sequence<OrderExpression> orders): this(null, orders) {}

		private RankingArguments(Sequence<Expression> partitions, Sequence<OrderExpression> orders): base() {
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