using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DeallocateStatement: CursorStatement {
		[Rule("<DeallocateStatement> ::= DEALLOCATE <GlobalOrLocalCursor>", ConstructorParameterMapping = new[] {1})]
		public DeallocateStatement(CursorName cursorName): base(cursorName) {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("DEALLOCATE ");
			writer.WriteScript(CursorName);
		}
	}
}