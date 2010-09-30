using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TriggerAllServerTarget: TriggerTarget {
		[Rule("<TriggerTarget> ::= ~ALL ~SERVER")]
		public TriggerAllServerTarget() {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("ALL SERVER");
		}
	}
}