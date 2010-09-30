using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class OuterApply: Join {
		[Rule("<Join> ::= ~OUTER ~APPLY <Source>")]
		public OuterApply(Source joinSource): base(joinSource) {}

		public override JoinKind Kind {
			get {
				return JoinKind.OuterApply;
			}
		}

		protected override string JoinSpecifier {
			get {
				return "OUTER APPLY";
			}
		}
	}
}