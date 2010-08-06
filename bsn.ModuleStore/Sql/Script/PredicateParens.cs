using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PredicateParens: Predicate {
		private readonly Predicate predicate;

		[Rule("<PredicateParens> ::= '(' <Predicate> ')'", ConstructorParameterMapping = new[] {1})]
		public PredicateParens(Predicate predicate) {
			if (predicate == null) {
				throw new ArgumentNullException("predicate");
			}
			PredicateParens parens = predicate as PredicateParens;
			this.predicate = parens != null ? parens.predicate : predicate;
		}

		public Predicate Predicate {
			get {
				return predicate;
			}
		}
	}
}