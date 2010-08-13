using System;
using System.Collections.Generic;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CursorDefinition: SqlScriptableToken {
		private readonly List<string> cursorOptions = new List<string>();
		private readonly UpdateMode cursorUpdate;
		private readonly bool global;
		private readonly SelectStatement selectStatement;

		[Rule("<CursorDefinition> ::= CURSOR <CursorOptionList> FOR <SelectStatement>", ConstructorParameterMapping = new[] {1, 3})]
		public CursorDefinition(Sequence<Identifier> cursorOptions, SelectStatement selectStatement): this(cursorOptions, selectStatement, null) {}

		[Rule("<CursorDefinition> ::= CURSOR <CursorOptionList> FOR <SelectStatement> <CursorUpdate>", ConstructorParameterMapping = new[] {1, 3, 4})]
		public CursorDefinition(Sequence<Identifier> cursorOptions, SelectStatement selectStatement, UpdateMode cursorUpdate) {
			this.selectStatement = selectStatement;
			this.cursorUpdate = cursorUpdate;
			foreach (Identifier cursorOption in cursorOptions) {
				string option = cursorOption.Value.ToUpperInvariant();
				this.cursorOptions.Add(option);
				if (option.Equals("GLOBAL", StringComparison.Ordinal)) {
					global = true;
				}
			}
		}

		public List<string> CursorOptions {
			get {
				return cursorOptions;
			}
		}

		public UpdateMode CursorUpdate {
			get {
				return cursorUpdate;
			}
		}

		public bool Global {
			get {
				return global;
			}
		}

		public SelectStatement SelectStatement {
			get {
				return selectStatement;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CURSOR");
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