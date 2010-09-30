using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CursorName: SqlQuotedName {
		private readonly bool global;

		[Rule("<CursorName> ::= Id")]
		[Rule("<CursorName> ::= QuotedId")]
		public CursorName(Identifier identifier): this(identifier.Value, false) {}

		[Rule("<GlobalOrLocalCursor> ::= <VariableName>")]
		public CursorName(VariableName variableName): this(variableName.Value, false) {}

		[Rule("<GlobalOrLocalCursor> ::= Id <CursorName>")]
		public CursorName(Identifier global, CursorName name): this(name.Value, true) {
			if (!string.Equals(global.Value, "GLOBAL", StringComparison.OrdinalIgnoreCase)) {
				throw new ArgumentException("GLOBAl expected", "global");
			}
		}

		private CursorName(string name, bool global): base(name) {
			this.global = global;
		}

		public bool Global {
			get {
				return global;
			}
		}

		protected internal override void WriteToInternal(SqlWriter writer, bool isPartOfQualifiedName) {
			Debug.Assert(!isPartOfQualifiedName);
			if (global) {
				writer.Write("GLOBAL ");
			}
			WriteNonGlobalInternal(writer);
		}

		internal CursorName AsGlobal() {
			if (global) {
				return this;
			}
			return new CursorName(Value, true);
		}

		internal void WriteNonGlobalInternal(SqlWriter writer) {
			base.WriteToInternal(writer, false);
		}
	}
}