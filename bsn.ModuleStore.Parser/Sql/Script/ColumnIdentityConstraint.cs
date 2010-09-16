using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnIdentityConstraint: ColumnConstraint {
		private readonly IntegerLiteral increment;
		private readonly IntegerLiteral seed;

		[Rule("<ColumnConstraint> ::= ~IDENTITY")]
		public ColumnIdentityConstraint() {}

		[Rule("<ColumnConstraint> ::= ~IDENTITY ~'(' IntegerLiteral ~',' IntegerLiteral ~')'")]
		public ColumnIdentityConstraint(IntegerLiteral seed, IntegerLiteral increment) {
			this.seed = seed;
			this.increment = increment;
		}

		public IntegerLiteral Increment {
			get {
				return increment;
			}
		}

		public IntegerLiteral Seed {
			get {
				return seed;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("IDENTITY");
			if (seed != null) {
				Debug.Assert(increment != null);
				writer.Write(" (");
				writer.WriteScript(seed, WhitespacePadding.None);
				writer.Write(", ");
				writer.WriteScript(increment, WhitespacePadding.None);
				writer.Write(')');
			}
		}
	}
}