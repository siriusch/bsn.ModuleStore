using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnNotNullableConstraint: ColumnConstraint {
		[Rule("<ColumnConstraint> ::= NOT NULL", AllowTruncationForConstructor = true)]
		public ColumnNotNullableConstraint() {}

		public override void WriteTo(TextWriter writer) {
			writer.Write("NOT NULL");
		}
	}
}