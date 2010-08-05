using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class FromClause: SqlToken {
		[Rule("<FromClause> ::= FROM <SourceRowset> <JoinChain>", ConstructorParameterMapping = new[] {1, 2})]
		public FromClause(SourceRowset sourceRowset, Sequence<Join> join) {}
	}
}