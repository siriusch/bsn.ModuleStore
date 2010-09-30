using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CrossApply: Join {
		[Rule("<Join> ::= ~CROSS ~APPLY <Source>")]
		public CrossApply(Source joinSource): base(joinSource) {}

		public override JoinKind Kind {
			get {
				return JoinKind.CrossApply;
			}
		}

		protected override string JoinSpecifier {
			get {
				return "CROSS APPLY";
			}
		}
	}
}