using System;
using System.IO;

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
			if (left == null) {
				throw new ArgumentNullException("left");
			}
			if (operation == null) {
				throw new ArgumentNullException("operation");
			}
			if (right == null) {
				throw new ArgumentNullException("right");
			}
			this.left = left;
			this.operation = operation;
			this.right = right;
		}

		public Expression Left {
			get {
				return left;
			}
		}
		public OperationToken Operation {
			get {
				return operation;
			}
		}
		public Expression Right {
			get {
				return right;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteScript(left);
			writer.WriteScript(operation);
			writer.WriteScript(right);
		}
	}
}