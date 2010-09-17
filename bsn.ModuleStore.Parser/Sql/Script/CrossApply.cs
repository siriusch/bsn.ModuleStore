using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CrossApply: Join {
		[Rule("<Join> ::= ~CROSS_APPLY <Source>")]
		public CrossApply(Source joinSource): base(joinSource) {}

		public override JoinKind Kind {
			get {
				return JoinKind.CrossApply;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CROSS APPLY ");
			base.WriteTo(writer);
		}
	}
}