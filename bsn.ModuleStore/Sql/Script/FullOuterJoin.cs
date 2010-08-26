using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class FullOuterJoin: PredicateJoin {
		[Rule("<Join> ::= FULL JOIN <Source> ON <Predicate>", ConstructorParameterMapping = new[] {2, 4})]
		[Rule("<Join> ::= FULL OUTER JOIN <Source> ON <Predicate>", ConstructorParameterMapping = new[] {3, 5})]
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