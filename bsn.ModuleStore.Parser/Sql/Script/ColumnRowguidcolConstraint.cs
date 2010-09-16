using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("ROWGUIDCOL")]
	public sealed class ColumnRowguidcolConstraint: ColumnConstraint {
		public ColumnRowguidcolConstraint() {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("ROWGUIDCOL");
		}
	}
}