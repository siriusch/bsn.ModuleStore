using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CrossJoin: Join {
		[Rule("<Join> ::= ~CROSS ~JOIN <Source>")]
		public CrossJoin(Source joinSource): base(joinSource) {}

		public override JoinKind Kind {
			get {
				return JoinKind.Cross;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CROSS ");
			base.WriteTo(writer);
		}
	}
}