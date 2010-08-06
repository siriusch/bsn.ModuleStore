using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class InnerJoin: PredicateJoin {
		[Rule("<Join> ::= JOIN <SourceRowset> ON <Predicate>", ConstructorParameterMapping = new[] {1, 3})]
		[Rule("<Join> ::= INNER JOIN <SourceRowset> ON <Predicate>", ConstructorParameterMapping = new[] {2, 4})]
		public InnerJoin(SourceRowset joinRowset, Predicate predicate): base(joinRowset, predicate) {}

		public override JoinKind Kind {
			get {
				return JoinKind.Inner;
			}
		}
	}
}