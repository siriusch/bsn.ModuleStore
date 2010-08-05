using System;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PredicateLike: PredicateNegable {
		[Rule("<PredicateLike> ::= <Expression> LIKE <CollableStringLiteral>", ConstructorParameterMapping = new[] {0, 2})]
		public PredicateLike(Expression value, Collable<StringLiteral> text): this(value, null, text, null) {}

		[Rule("<PredicateLike> ::= <Expression> NOT LIKE <CollableStringLiteral>", ConstructorParameterMapping = new[] {0, 1, 3})]
		public PredicateLike(Expression value, IToken not, Collable<StringLiteral> text): this(value, not, text, null) {}

		[Rule("<PredicateLike> ::= <Expression> LIKE <CollableStringLiteral> ESCAPE StringLiteral", ConstructorParameterMapping = new[] {0, 2, 4})]
		public PredicateLike(Expression value, Collable<StringLiteral> text, StringLiteral escape): this(value, null, text, escape) {}

		[Rule("<PredicateLike> ::= <Expression> NOT LIKE <CollableStringLiteral> ESCAPE StringLiteral", ConstructorParameterMapping = new[] {0, 1, 3, 5})]
		public PredicateLike(Expression value, IToken not, Collable<StringLiteral> text, StringLiteral escape): base(not != null) {}
	}
}