using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class FullOuterJoin: PredicateJoin {
		[Rule("<Join> ::= ~FULL ~JOIN <Source> ~ON <Predicate>")]
		[Rule("<Join> ::= ~FULL ~OUTER ~JOIN <Source> ~ON <Predicate>")]
		public FullOuterJoin(Source joinSource, Predicate predicate): base(joinSource, predicate) {}

		public override JoinKind Kind {
			get {
				return JoinKind.Full;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("FULL ");
			base.WriteTo(writer);
		}
	}
}