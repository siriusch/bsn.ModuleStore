using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CloseStatement: CursorStatement {
		[Rule("<CloseStatement> ::= CLOSE <GlobalOrLocalCursor>", ConstructorParameterMapping = new[] {1})]
		public CloseStatement(CursorName cursorName): base(cursorName) {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CLOSE ");
			writer.WriteScript(CursorName);
		}
	}
}