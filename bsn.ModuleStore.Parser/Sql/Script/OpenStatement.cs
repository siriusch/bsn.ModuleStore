using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class OpenStatement: CursorStatement {
		[Rule("<OpenStatement> ::= ~OPEN <GlobalOrLocalCursor>")]
		public OpenStatement(CursorName cursorName): base(cursorName) {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("OPEN ");
			writer.WriteScript(CursorName, WhitespacePadding.None);
		}
	}
}