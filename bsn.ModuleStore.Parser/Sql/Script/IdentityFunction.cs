using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class IdentityFunction: FunctionCall {
		private readonly IntegerLiteral increment;
		private readonly IntegerLiteral seed;
		private readonly TypeName typeName;

		[Rule("<FunctionCall> ::= ~IDENTITY ~'(' <TypeName> ~')'")]
		public IdentityFunction(TypeName typeName): this(typeName, null, null) {}

		[Rule("<FunctionCall> ::= ~IDENTITY ~'(' <TypeName> ~',' <IntegerLiteral> ~',' <IntegerLiteral> ~')'")]
		public IdentityFunction(TypeName typeName, IntegerLiteral seed, IntegerLiteral increment): base() {
			Debug.Assert(typeName != null);
			this.typeName = typeName;
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

		public TypeName TypeName {
			get {
				return typeName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("IDENTITY(");
			writer.WriteScript(typeName, WhitespacePadding.None);
			writer.WriteScript(seed, WhitespacePadding.None, ", ", null);
			writer.WriteScript(increment, WhitespacePadding.None, ", ", null);
			writer.Write(')');
		}
	}
}