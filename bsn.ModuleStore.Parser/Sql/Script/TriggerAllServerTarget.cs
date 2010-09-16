using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("ALL_SERVER")]
	public sealed class TriggerAllServerTarget: TriggerTarget {
		public TriggerAllServerTarget() {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("ALL SERVER");
		}
	}
}