using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class RankingArguments: SqlToken {
		private readonly Sequence<Order> orders;
		private readonly Sequence<Expression> partitions;

		[Rule("<RankingArguments> ::= PARTITION_BY <ExpressionList> <OptionalOrderClause>", ConstructorParameterMapping = new[] {1, 2})]
		public RankingArguments(Sequence<Expression> partitions, Optional<Sequence<Order>> orders): this(orders) {
			this.partitions = partitions;
		}

		[Rule("<RankingArguments> ::= <OrderClause>")]
		private RankingArguments(Sequence<Order> orders) {
			this.orders = orders;
		}
	}
}