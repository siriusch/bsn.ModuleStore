using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ExpressionWithFunctionCalls: ExpressionFunctionCalls {
		private readonly Expression expression;

		[Rule("<Value> ::= <ExpressionParens> ~'.' <NamedFunctionList>")]
		public ExpressionWithFunctionCalls(Expression expression, Sequence<NamedFunction> functions): base(functions.Item, functions.Next) {
			Debug.Assert(expression != null);
			this.expression = expression;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		protected override void WriteToInternal(SqlWriter writer) {
			writer.WriteScript(expression, WhitespacePadding.None);
			writer.Write('.');
			base.WriteToInternal(writer);
		}
	}
}