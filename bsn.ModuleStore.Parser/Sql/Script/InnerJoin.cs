using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class InnerJoin: PredicateJoin {
		[Rule("<Join> ::= ~JOIN <Source> ~ON <Predicate>")]
		[Rule("<Join> ::= ~INNER ~JOIN <Source> ~ON <Predicate>")]
		public InnerJoin(Source joinSource, Predicate predicate): base(joinSource, predicate) {}

		public override JoinKind Kind {
			get {
				return JoinKind.Inner;
			}
		}
	}
}