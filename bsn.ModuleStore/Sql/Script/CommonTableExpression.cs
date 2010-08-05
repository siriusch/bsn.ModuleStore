using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CommonTableExpression: SqlToken {
		[Rule("<CTE> ::= <AliasName> AS '(' <SelectQuery> ')'", ConstructorParameterMapping = new[] {0, 3})]
		public CommonTableExpression(AliasName aliasName, SelectQuery selectQuery) {}
	}
}