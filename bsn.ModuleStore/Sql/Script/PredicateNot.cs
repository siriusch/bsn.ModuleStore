using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PredicateNot: Predicate {
		private readonly Predicate predicate;

		[Rule("<PredicateNot> ::= NOT <PredicateBetween>", ConstructorParameterMapping = new[] {1})]
		public PredicateNot(Predicate predicate) {
			this.predicate = predicate;
		}
	}
}