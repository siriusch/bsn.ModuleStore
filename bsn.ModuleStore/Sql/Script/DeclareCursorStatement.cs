using System;
using System.Collections.Generic;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class DeclareCursorStatement: SqlCursorStatement {
		private static readonly Identifier globalIdentifier = new Identifier("GLOBAL");

		private readonly SelectStatement selectStatement;
		private readonly UpdateMode cursorUpdate;
		private readonly List<string> cursorOptions;

		[Rule("<DeclareStatement> ::= DECLARE <CursorName> CURSOR <CursorOptionList> FOR <SelectStatement> <CursorUpdate>", ConstructorParameterMapping=new[] { 1, 3, 5, 6 })]
		public DeclareCursorStatement(CursorName cursorName, Sequence<Identifier> cursorOptions, SelectStatement selectStatement, UpdateMode cursorUpdate): base(cursorOptions.Contains(globalIdentifier) ? cursorName.AsGlobal() : cursorName) {
			if (selectStatement == null) {
				throw new ArgumentNullException("selectStatement");
			}
			this.selectStatement = selectStatement;
			this.cursorUpdate = cursorUpdate;
			this.cursorOptions = cursorOptions.Where(identifier => !identifier.Equals(globalIdentifier)).Select(identifier => identifier.Value).ToList();
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("DECLARE ");
			writer.Write(CursorName.Value);
			writer.Write(" CURSOR");
			if (CursorName.Global) {
				writer.Write(" GLOBAL");
			}
			foreach (string cursorOption in cursorOptions) {
				writer.Write(' ');
				writer.Write(cursorOption);
			}
			writer.Write(" FOR ");
			selectStatement.WriteTo(writer);
			if (cursorUpdate != null) {
				writer.Write(' ');
				cursorUpdate.WriteTo(writer);
			}
		}
	}

	public class SelectStatement: SqlStatement {}
}