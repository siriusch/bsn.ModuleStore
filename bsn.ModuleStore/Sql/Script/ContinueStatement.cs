using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ContinueStatement: SqlStatement {
		[Rule("<ContinueStatement> ::= CONTINUE", AllowTruncationForConstructor = true)]
		public ContinueStatement() {}

		public override void WriteTo(TextWriter writer) {
			writer.Write("CONTINUE");
		}
	}
}