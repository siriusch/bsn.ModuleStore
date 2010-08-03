using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class DeallocateStatement: SqlCursorStatement {
		[Rule("<DeallocateStatement> ::= DEALLOCATE <GlobalOrLocalCursor>", ConstructorParameterMapping = new[] {1})]
		public DeallocateStatement(CursorName cursorName): base(cursorName) {}

		public override void WriteTo(TextWriter writer) {
			writer.Write("DEALLOCATE ");
			base.WriteTo(writer);
		}
	}
}