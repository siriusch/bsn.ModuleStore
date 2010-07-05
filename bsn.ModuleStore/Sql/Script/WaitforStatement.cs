using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class WaitforStatement: SqlStatement {
		[Rule("<WaitforStatement> ::= WAITFOR Id <StringValue>", ConstructorParameterMapping = new[] {1, 2})]
		public WaitforStatement(Identifier identifier, SqlToken stringValue) {}
	}
}