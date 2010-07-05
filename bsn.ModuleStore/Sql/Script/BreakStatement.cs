using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class BreakStatement: SqlStatement {
		[Rule("<BreakStatement> ::= BREAK", AllowTruncationForConstructor = true)]
		public BreakStatement() {}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("BREAK");
		}
	}
}