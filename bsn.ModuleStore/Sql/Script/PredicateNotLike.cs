using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PredicateNotLike: PredicateLike {
		[Rule("<PredicateLike> ::= <Expression> NOT LIKE <CollableStringLiteral>", ConstructorParameterMapping = new[] {0, 3})]
		public PredicateNotLike(Expression value, StringLiteral text) : this(value, text, null) {
		}

		[Rule("<PredicateLike> ::= <Expression> NOT LIKE <CollableStringLiteral> ESCAPE StringLiteral", ConstructorParameterMapping=new[] { 0, 3, 5 })]
		public PredicateNotLike(Expression value, StringLiteral text, StringLiteral escape): base(value, true, text, escape) {}
	}
}