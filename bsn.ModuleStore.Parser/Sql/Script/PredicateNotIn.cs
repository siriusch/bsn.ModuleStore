using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PredicateNotIn: PredicateIn {
		[Rule("<PredicateIn> ::= <Expression> ~NOT ~IN <Tuple>")]
		public PredicateNotIn(Expression value, Tuple tuple): base(value, true, tuple) {}
	}
}