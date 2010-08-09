using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PredicateNotIn: PredicateIn {
		[Rule("<PredicateIn> ::= <Expression> NOT IN <Tuple>", ConstructorParameterMapping=new[] { 0, 3 })]
		public PredicateNotIn(Expression value, Tuple tuple) : base(value, true, tuple) {
		}
	}
}