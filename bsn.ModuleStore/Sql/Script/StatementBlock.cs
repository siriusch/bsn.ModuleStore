using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class StatementBlock: SqlStatement {
		private readonly Sequence<SqlStatement> statements;

		[Rule("<StatementBlock> ::= BEGIN <StatementList> END", ConstructorParameterMapping = new[] {1})]
		public StatementBlock(Sequence<SqlStatement> statements) {
			this.statements = statements;
		}

		public Sequence<SqlStatement> Statements {
			get {
				return statements;
			}
		}
	}
}