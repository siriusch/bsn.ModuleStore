using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PrintStatement: Statement {
		private readonly Expression expression;

		[Rule("<PrintStatement> ::= PRINT <Expression>", ConstructorParameterMapping = new[] {1})]
		public PrintStatement(Expression expression) {
			Debug.Assert(expression != null);
			this.expression = expression;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("PRINT ");
			writer.WriteScript(expression, WhitespacePadding.None);
		}
	}
}