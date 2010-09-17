using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionNullifFunction: ExpressionFunction {
		private readonly Expression firstExpression;
		private readonly Expression secondExpression;

		[Rule("<Value> ::= ~NULLIF ~'(' <Expression> ~',' <Expression> ~')'")]
		public ExpressionNullifFunction(Expression firstExpression, Expression secondExpression) {
			Debug.Assert(firstExpression != null);
			Debug.Assert(secondExpression != null);
			this.firstExpression = firstExpression;
			this.secondExpression = secondExpression;
		}

		public Expression FirstExpression {
			get {
				return firstExpression;
			}
		}

		public Expression SecondExpression {
			get {
				return secondExpression;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("NULLIF(");
			writer.WriteScript(firstExpression, WhitespacePadding.None);
			writer.Write(", ");
			writer.WriteScript(secondExpression, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}