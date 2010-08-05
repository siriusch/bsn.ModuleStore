using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PredicateExists: Predicate {
		[Rule("<PredicateExists> ::= EXISTS '(' <SelectQuery> ')'", ConstructorParameterMapping = new[] {2})]
		public PredicateExists(SelectQuery selectQuery) {}
	}
}