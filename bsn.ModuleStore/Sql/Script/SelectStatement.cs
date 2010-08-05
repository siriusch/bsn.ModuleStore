using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class SelectStatement: SqlStatement {
		[Rule("<SelectStatement> ::= <CTEGroup> <SelectQuery> <ForClause>")]
		public SelectStatement(Optional<Sequence<CommonTableExpression>> ctes, SelectQuery selectQuery, ForClause forClause) {}
	}
}