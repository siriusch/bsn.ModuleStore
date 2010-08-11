using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionBinaryOperation: Expression {
		private readonly Expression left;
		private readonly OperationToken operation;
		private readonly Expression right;

		[Rule("<ExpressionAdd> ::= <ExpressionMult> '+' <ExpressionAdd>")]
		[Rule("<ExpressionAdd> ::= <ExpressionMult> '-' <ExpressionAdd>")]
		[Rule("<ExpressionMult> ::= <ExpressionMult> '*' <ExpressionNegate>")]
		[Rule("<ExpressionMult> ::= <ExpressionMult> '/' <ExpressionNegate>")]
		[Rule("<ExpressionMult> ::= <ExpressionMult> '%' <ExpressionNegate>")]
		public ExpressionBinaryOperation(Expression left, OperationToken operation, Expression right) {
			Debug.Assert(left != null);
			Debug.Assert(operation != null);
			Debug.Assert(right != null);
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