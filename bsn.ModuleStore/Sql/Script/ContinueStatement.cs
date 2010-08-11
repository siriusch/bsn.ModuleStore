using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ContinueStatement: Statement {
		[Rule("<ContinueStatement> ::= CONTINUE", AllowTruncationForConstructor = true)]
		public ContinueStatement() {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CONTINUE");
		}
	}
}