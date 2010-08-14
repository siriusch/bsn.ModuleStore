using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class LeftOuterJoin: PredicateJoin {
		[Rule("<Join> ::= LEFT JOIN <Source> ON <Predicate>", ConstructorParameterMapping = new[] {2, 4})]
		[Rule("<Join> ::= LEFT OUTER JOIN <Source> ON <Predicate>", ConstructorParameterMapping = new[] {3, 5})]
		public LeftOuterJoin(Source joinSource, Predicate predicate) : base(joinSource, predicate) {
		}

		public override JoinKind Kind {
			get {
				return JoinKind.Left;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("LEFT ");
			base.WriteTo(writer);
		}
	}
}