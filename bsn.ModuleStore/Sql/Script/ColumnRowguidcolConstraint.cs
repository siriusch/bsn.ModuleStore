using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnRowguidcolConstraint: ColumnConstraint {
		[Rule("<ColumnConstraint> ::= ROWGUIDCOL", AllowTruncationForConstructor = true)]
		public ColumnRowguidcolConstraint() {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("ROWGUIDCOL");
		}
	}
}