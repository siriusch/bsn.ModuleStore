using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CastFunction: FunctionCall {
		private readonly Expression expression;
		private readonly TypeName typeName;

		[Rule("<FunctionCall> ::= ~CAST ~'(' <Expression> ~AS <TypeName> ~')'")]
		public CastFunction(Expression expression, TypeName typeName) {
			Debug.Assert(expression != null);
			Debug.Assert(typeName != null);
			this.expression = expression;
			this.typeName = typeName;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public TypeName TypeName {
			get {
				return typeName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CAST(");
			writer.WriteScript(expression, WhitespacePadding.None);
			writer.Write(" AS ");
			writer.WriteScript(typeName, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}