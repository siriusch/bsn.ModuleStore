using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CloseStatement: SqlCursorStatement {
		[Rule("<CloseStatement> ::= CLOSE <GlobalOrLocalCursor>", ConstructorParameterMapping = new[] {1})]
		public CloseStatement(CursorName cursorName): base(cursorName) {}

		public override void WriteTo(TextWriter writer) {
			writer.Write("CLOSE ");
			base.WriteTo(writer);
		}
	}
}