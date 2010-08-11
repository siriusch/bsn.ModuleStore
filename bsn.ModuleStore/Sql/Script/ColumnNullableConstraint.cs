using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnNullableConstraint: ColumnConstraint {
		[Rule("<ColumnConstraint> ::= NULL", AllowTruncationForConstructor = true)]
		public ColumnNullableConstraint() {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("NULL");
		}
	}
}