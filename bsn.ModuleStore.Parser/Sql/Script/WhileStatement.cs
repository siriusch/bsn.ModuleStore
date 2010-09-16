using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class WhileStatement: Statement {
		private readonly Expression expression;
		private readonly Statement statement;

		[Rule("<WhileStatement> ::= ~WHILE <Expression> <StatementGroup>")]
		public WhileStatement(Expression expression, Statement statement) {
			Debug.Assert(expression != null);
			Debug.Assert(statement != null);
			this.expression = expression;
			this.statement = statement;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public Statement Statement {
			get {
				return statement;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("WHILE ");
			writer.WriteScript(expression, WhitespacePadding.SpaceAfter);
			writer.WriteScript(statement, WhitespacePadding.None);
		}
	}
}