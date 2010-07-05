using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class WhileStatement: SqlStatement {
		[Rule("<WhileStatement> ::= WHILE <Expression> <StatementGroup>", ConstructorParameterMapping = new[] {1, 2})]
		public WhileStatement(Expression expression, SqlStatement statement) {}
	}
}