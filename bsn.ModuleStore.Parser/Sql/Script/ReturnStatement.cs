using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ReturnStatement: Statement {
		private readonly Expression expression;

		[Rule("<ReturnStatement> ::= ~RETURN")]
		public ReturnStatement(): this(null) {}

		[Rule("<ReturnStatement> ::= ~RETURN <Expression>")]
		public ReturnStatement(Expression expression) {
			this.expression = expression;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("RETURN");
			writer.WriteScript(expression, WhitespacePadding.SpaceBefore);
		}
	}
}