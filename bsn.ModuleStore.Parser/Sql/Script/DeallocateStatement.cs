using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DeallocateStatement: CursorStatement {
		[Rule("<DeallocateStatement> ::= ~DEALLOCATE <GlobalOrLocalCursor>")]
		public DeallocateStatement(CursorName cursorName): base(cursorName) {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("DEALLOCATE ");
			writer.WriteScript(CursorName, WhitespacePadding.None);
		}
	}
}