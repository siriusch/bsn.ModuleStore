using System;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class UnionClause: SqlToken {
		private readonly bool all;
		private readonly SelectQuery selectQuery;

		[Rule("<UnionClause> ::=")]
		public UnionClause() {}

		[Rule("<UnionClause> ::= UNION <SelectQuery>")]
		[Rule("<UnionClause> ::= UNION ALL <SelectQuery>", ConstructorParameterMapping = new[] {1, 2})]
		public UnionClause(IToken all, SelectQuery selectQuery): this() {
			this.selectQuery = selectQuery;
			this.all = all.NameIs("ALL");
		}
	}
}