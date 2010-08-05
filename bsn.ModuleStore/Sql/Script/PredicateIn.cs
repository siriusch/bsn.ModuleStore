using System;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PredicateIn: PredicateNegable {
		[Rule("<PredicateIn> ::= <Expression> IN <Tuple>", ConstructorParameterMapping = new[] {0, 2})]
		public PredicateIn(Expression value, Tuple tuple): this(value, null, tuple) {}

		[Rule("<PredicateIn> ::= <Expression> NOT IN <Tuple>", ConstructorParameterMapping = new[] {0, 1, 3})]
		public PredicateIn(Expression value, IToken not, Tuple tuple): base(not != null) {}
	}
}