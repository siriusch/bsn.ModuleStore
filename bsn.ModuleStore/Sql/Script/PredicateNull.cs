using System;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PredicateNull: PredicateNegable {
		private readonly Expression value;

		[Rule("<PredicateNull> ::= <Expression> IS NULL", AllowTruncationForConstructor = true)]
		public PredicateNull(Expression value): this(value, null) {}

		[Rule("<PredicateNull> ::= <Expression> IS NOT NULL", ConstructorParameterMapping = new[] {0, 2})]
		public PredicateNull(Expression value, IToken not): base(not != null) {
			this.value = value;
		}
	}
}