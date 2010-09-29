using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class LeftOuterJoin: PredicateJoin {
		[Rule("<Join> ::= ~LEFT ~JOIN <Source> ~ON <Predicate>")]
		[Rule("<Join> ::= ~LEFT ~OUTER ~JOIN <Source> ~ON <Predicate>")]
		public LeftOuterJoin(Source joinSource, Predicate predicate): base(joinSource, predicate) {}

		public override JoinKind Kind {
			get {
				return JoinKind.Left;
			}
		}

		protected override string JoinSpecifier {
			get {
				return "LEFT JOIN";
			}
		}
	}
}