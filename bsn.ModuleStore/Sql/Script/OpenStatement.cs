using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class OpenStatement: SqlCursorStatement {
		[Rule("<OpenStatement> ::= OPEN <GlobalOrLocalCursor>", ConstructorParameterMapping = new[] {1})]
		public OpenStatement(CursorName cursorName): base(cursorName) {}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("OPEN ");
			base.WriteTo(writer);
		}
	}
}