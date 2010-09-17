using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ContinueStatement: Statement {
		[Rule("<ContinueStatement> ::= ~CONTINUE")]
		public ContinueStatement() {}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("CONTINUE");
		}
	}
}