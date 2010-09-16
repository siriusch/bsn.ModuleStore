using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionParens: Expression {
		private readonly Expression expression;

		[Rule("<ExpressionParens> ::= ~'(' <Expression> ~')'")]
		public ExpressionParens(Expression expression) {
			Debug.Assert(expression != null);
			ExpressionParens parens = expression as ExpressionParens;
			this.expression = (parens != null) ? parens.expression : expression;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write('(');
			writer.WriteScript(expression, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}