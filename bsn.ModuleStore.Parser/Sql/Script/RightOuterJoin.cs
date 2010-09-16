using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class RightOuterJoin: PredicateJoin {
		[Rule("<Join> ::= ~RIGHT ~JOIN <Source> ~ON <Predicate>")]
		[Rule("<Join> ::= ~RIGHT ~OUTER ~JOIN <Source> ~ON <Predicate>")]
		public RightOuterJoin(Source joinSource, Predicate predicate): base(joinSource, predicate) {}

		public override JoinKind Kind {
			get {
				return JoinKind.Right;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("RIGHT ");
			base.WriteTo(writer);
		}
	}
}