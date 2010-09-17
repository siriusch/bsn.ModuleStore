using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class OuterApply: Join {
		[Rule("<Join> ::= ~OUTER_APPLY <Source>")]
		public OuterApply(Source joinSource): base(joinSource) {}

		public override JoinKind Kind {
			get {
				return JoinKind.OuterApply;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("OUTER APPLY ");
			base.WriteTo(writer);
		}
	}
}