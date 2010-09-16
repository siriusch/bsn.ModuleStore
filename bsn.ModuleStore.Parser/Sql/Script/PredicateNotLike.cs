using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PredicateNotLike: PredicateLike {
		[Rule("<PredicateLike> ::= <Expression> ~NOT ~LIKE <CollableStringLiteral>")]
		public PredicateNotLike(Expression value, StringLiteral text): this(value, text, null) {}

		[Rule("<PredicateLike> ::= <Expression> ~NOT ~LIKE <CollableStringLiteral> ~ESCAPE StringLiteral")]
		public PredicateNotLike(Expression value, StringLiteral text, StringLiteral escape): base(value, true, text, escape) {}
	}
}