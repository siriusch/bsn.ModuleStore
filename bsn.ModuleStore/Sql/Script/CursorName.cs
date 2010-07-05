using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CursorName: SqlName {
		private readonly bool global;

		private CursorName(string name, bool global): base(name) {
			this.global = global;
		}

		[Rule("<CursorName> ::= Id")]
		public CursorName(Identifier identifier): this(identifier.Value, false) {}

		[Rule("<GlobalOrLocalCursor> ::= <VariableName>")]
		public CursorName(VariableName variableName) : this(variableName.Value, false) {
		}

		[Rule("<CursorName> ::= Id")]
		public CursorName(Identifier global, CursorName name) : this(name.Value, true) {
			if (!string.Equals(global.Value, "GLOBAL", StringComparison.OrdinalIgnoreCase)) {
				throw new ArgumentException("GLOBAl expected", "global");
			}
		}

		public bool Global {
			get {
				return global;
			}
		}

		internal CursorName AsGlobal() {
			if (global) {
				return this;
			}
			return new CursorName(Value, true);
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			if (global) {
				writer.Write("GLOBAL ");
			}
			base.WriteTo(writer);
		}
	}
}