using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CloseStatement: SqlCursorStatement {
		[Rule("<CloseStatement> ::= CLOSE <GlobDealOrLocalCursor>", ConstructorParameterMapping = new[] {1})]
		public CloseStatement(CursorName cursorName): base(cursorName) {}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("CLOSE ");
			base.WriteTo(writer);
		}
	}
}