using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class InnerJoin: PredicateJoin {
		[Rule("<Join> ::= JOIN <Source> ON <Predicate>", ConstructorParameterMapping = new[] {1, 3})]
		[Rule("<Join> ::= INNER JOIN <Source> ON <Predicate>", ConstructorParameterMapping = new[] {2, 4})]
		public InnerJoin(Source joinSource, Predicate predicate) : base(joinSource, predicate) {
		}

		public override JoinKind Kind {
			get {
				return JoinKind.Inner;
			}
		}
	}
}