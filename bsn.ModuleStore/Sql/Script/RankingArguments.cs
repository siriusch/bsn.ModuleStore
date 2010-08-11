using System;
using System.Collections.Generic;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class RankingArguments: SqlToken, IScriptable {
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

		public List<OrderExpression> Orders {
			get {
				return orders;
			}
		}

		public List<Expression> Partitions {
			get {
				return partitions;
			}
		}

		public void WriteTo(SqlWriter writer) {
			if (partitions.Count > 0) {
				writer.Write("PARTITION BY ");
				writer.WriteSequence(partitions, WhitespacePadding.None, ", ");
				if (orders.Count > 0) {
					writer.Write(' ');
				}
			}
			if (orders.Count > 0) {
				writer.Write("ORDER BY ");
				writer.WriteSequence(orders, WhitespacePadding.None, ", ");
			}
		}
	}
}