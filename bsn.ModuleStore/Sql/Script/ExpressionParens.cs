using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionParens: Expression {
		private readonly Expression expression;

		[Rule("<ExpressionParens> ::= '(' <Expression> ')'", ConstructorParameterMapping = new[] {1})]
		public ExpressionParens(Expression expression) {
			if (expression == null) {
				throw new ArgumentNullException("expression");
			}
			ExpressionParens parens = expression as ExpressionParens;
			this.expression = (parens != null) ? parens.expression : expression;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}
	}
}