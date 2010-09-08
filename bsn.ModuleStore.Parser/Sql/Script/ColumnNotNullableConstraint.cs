using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnNotNullableConstraint: ColumnConstraint {
		[Rule("<ColumnConstraint> ::= NOT NULL", AllowTruncationForConstructor = true)]
		public ColumnNotNullableConstraint() {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("NOT NULL");
		}
	}
}