using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ExpressionBinaryOperation: Expression {
		private readonly Expression left;
		private readonly OperationToken operation;
		private readonly Expression right;

		[Rule("<ExpressionAdd> ::= <ExpressionMult> '+' <ExpressionAdd>")]
		[Rule("<ExpressionAdd> ::= <ExpressionMult> '-' <ExpressionAdd>")]
		[Rule("<ExpressionMult> ::= <ExpressionMult> '*' <ExpressionNegate>")]
		[Rule("<ExpressionMult> ::= <ExpressionMult> '/' <ExpressionNegate>")]
		[Rule("<ExpressionMult> ::= <ExpressionMult> '%' <ExpressionNegate>")]
		public ExpressionBinaryOperation(Expression left, OperationToken operation, Expression right) {
			this.left = left;
			this.operation = operation;
			this.right = right;
		}
	}
}