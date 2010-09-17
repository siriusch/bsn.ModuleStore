using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class BreakStatement: Statement {
		[Rule("<BreakStatement> ::= ~BREAK")]
		public BreakStatement() {}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("BREAK");
		}
	}
}