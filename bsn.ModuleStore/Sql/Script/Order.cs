using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class Order: SqlToken {
		[Rule("<Order> ::= <Expression> <OrderType>")]
		public Order(Expression expression, OrderType oderType) {}
	}
}