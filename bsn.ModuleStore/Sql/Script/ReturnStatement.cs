using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ReturnStatement: Statement {
		private readonly Expression expression;

		[Rule("<ReturnStatement> ::= RETURN", AllowTruncationForConstructor = true)]
		public ReturnStatement(): this(null) {}

		[Rule("<ReturnStatement> ::= RETURN <Expression>", ConstructorParameterMapping = new[] {1})]
		public ReturnStatement(Expression expression) {
			this.expression = expression;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("RETURN");
			writer.WriteScript(expression, " ", null);
		}
	}
}