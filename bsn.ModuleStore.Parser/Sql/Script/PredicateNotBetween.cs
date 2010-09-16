using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PredicateNotBetween: PredicateBetween {
		[Rule("<PredicateBetween> ::= <Expression> ~NOT ~BETWEEN <Expression> ~AND <Expression>")]
		public PredicateNotBetween(Expression valueExpression, Expression min, Expression max): base(valueExpression, true, min, max) {}
	}
}