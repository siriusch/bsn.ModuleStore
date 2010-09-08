using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class OpenStatement: CursorStatement {
		[Rule("<OpenStatement> ::= OPEN <GlobalOrLocalCursor>", ConstructorParameterMapping = new[] {1})]
		public OpenStatement(CursorName cursorName): base(cursorName) {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("OPEN ");
			writer.WriteScript(CursorName, WhitespacePadding.None);
		}
	}
}