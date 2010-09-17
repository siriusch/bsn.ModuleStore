using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class WhileStatement: Statement {
		private readonly Predicate predicate;
		private readonly Statement statement;

		[Rule("<WhileStatement> ::= ~WHILE <Predicate> <StatementGroup>")]
		public WhileStatement(Predicate predicate, Statement statement) {
			Debug.Assert(predicate != null);
			Debug.Assert(statement != null);
			this.predicate = predicate;
			this.statement = statement;
		}

		public Predicate Predicate {
			get {
				return predicate;
			}
		}

		public Statement Statement {
			get {
				return statement;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("WHILE ");
			writer.WriteScript(predicate, WhitespacePadding.SpaceAfter);
			writer.WriteScript(statement, WhitespacePadding.None);
		}
	}
}