using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionNegate: Expression {
		private readonly Expression expression;

		[Rule("<ExpressionNegate> ::= '-' <ExpressionFunction>", ConstructorParameterMapping = new[] {1})]
		public ExpressionNegate(Expression expression) {
			if (expression == null) {
				throw new ArgumentNullException("expression");
			}
			this.expression = expression;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write('-');
			writer.WriteScript(expression);
		}
	}
}