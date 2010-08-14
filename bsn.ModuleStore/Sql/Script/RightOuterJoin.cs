using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class RightOuterJoin: PredicateJoin {
		[Rule("<Join> ::= RIGHT JOIN <Source> ON <Predicate>", ConstructorParameterMapping = new[] {2, 4})]
		[Rule("<Join> ::= RIGHT OUTER JOIN <Source> ON <Predicate>", ConstructorParameterMapping = new[] {3, 5})]
		public RightOuterJoin(Source joinSource, Predicate predicate) : base(joinSource, predicate) {
		}

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