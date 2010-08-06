using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ExpressionNegate: Expression {
		private readonly Expression expression;

		[Rule("<ExpressionNegate> ::= '-' <ExpressionFunction>", ConstructorParameterMapping = new[] {1})]
		public ExpressionNegate(Expression expression) {
			if (expression == null) {
				throw new ArgumentNullException("expression");
			}
			this.expression = expression;
		}
	}
}