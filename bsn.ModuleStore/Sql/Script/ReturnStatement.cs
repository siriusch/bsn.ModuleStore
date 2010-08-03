using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ReturnStatement: SqlStatement {
		private readonly Expression expression;

		[Rule("<ReturnStatement> ::= RETURN", AllowTruncationForConstructor = true)]
		public ReturnStatement(): this(null) {}

		[Rule("<ReturnStatement> ::= RETURN <Expression>", ConstructorParameterMapping = new[] {1})]
		public ReturnStatement(Expression expression) {
			this.expression = expression;
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("RETURN");
			if (expression != null) {
				writer.Write(' ');
				expression.WriteTo(writer);
			}
		}
	}
}