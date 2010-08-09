using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class BreakStatement: SqlStatement {
		[Rule("<BreakStatement> ::= BREAK", AllowTruncationForConstructor = true)]
		public BreakStatement() {}

		public override void WriteTo(TextWriter writer) {
			writer.Write("BREAK");
		}
	}
}