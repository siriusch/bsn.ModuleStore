using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TriggerDatabaseTarget: TriggerTarget {
		[Rule("<TriggerTarget> ::= DATABASE", AllowTruncationForConstructor = true)]
		public TriggerDatabaseTarget() {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("DATABASE");
		}
	}
}