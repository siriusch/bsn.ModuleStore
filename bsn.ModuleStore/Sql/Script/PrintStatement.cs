using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PrintStatement: SqlStatement {
		private readonly Expression expression;

		[Rule("<PrintStatement> ::= PRINT <Expression>", ConstructorParameterMapping = new[] {1})]
		public PrintStatement(Expression expression) {
			if (expression == null) {
				throw new ArgumentNullException("expression");
			}
			this.expression = expression;
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("PRINT ");
			expression.WriteTo(writer);
		}
	}
}