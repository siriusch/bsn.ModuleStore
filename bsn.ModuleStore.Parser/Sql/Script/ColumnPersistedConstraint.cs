using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnPersistedConstraint: ColumnConstraint {
		[Rule("<ComputedColumnConstraint> ::= ~PERSISTED")]
		public ColumnPersistedConstraint() {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("PERSISTED");
		}
	}
}