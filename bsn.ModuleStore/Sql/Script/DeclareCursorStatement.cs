using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DeclareCursorStatement: CursorStatement {
		private static readonly Identifier globalIdentifier = new Identifier("GLOBAL");

		private readonly List<string> cursorOptions;
		private readonly UpdateMode cursorUpdate;
		private readonly SelectStatement selectStatement;

		[Rule("<DeclareStatement> ::= DECLARE <CursorName> CURSOR <CursorOptionList> FOR <SelectStatement>", ConstructorParameterMapping = new[] {1, 3, 5})]
		public DeclareCursorStatement(CursorName cursorName, Sequence<Identifier> cursorOptions, SelectStatement selectStatement): this(cursorName, cursorOptions, selectStatement, null) {}

		[Rule("<DeclareStatement> ::= DECLARE <CursorName> CURSOR <CursorOptionList> FOR <SelectStatement> <CursorUpdate>", ConstructorParameterMapping = new[] {1, 3, 5, 6})]
		public DeclareCursorStatement(CursorName cursorName, Sequence<Identifier> cursorOptions, SelectStatement selectStatement, UpdateMode cursorUpdate): base(cursorOptions.Contains(globalIdentifier) ? cursorName.AsGlobal() : cursorName) {
			Debug.Assert(selectStatement != null);
			Debug.Assert(cursorOptions != null);
			this.selectStatement = selectStatement;
			this.cursorUpdate = cursorUpdate;
			this.cursorOptions = cursorOptions.Where(identifier => !identifier.Equals(globalIdentifier)).Select(identifier => identifier.Value).ToList();
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("DECLARE ");
			CursorName.WriteNonGlobalInternal(writer);
			writer.Write(" CURSOR");
			if (CursorName.Global) {
				writer.Write(" GLOBAL");
			}
			foreach (string cursorOption in cursorOptions) {
				writer.Write(' ');
				writer.Write(cursorOption);
			}
			writer.Write(" FOR ");
			writer.WriteScript(selectStatement, WhitespacePadding.None);
			writer.WriteScript(cursorUpdate, WhitespacePadding.SpaceBefore);
		}
	}
}