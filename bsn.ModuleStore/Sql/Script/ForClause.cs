using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ForClause: SqlToken {
		[Rule("<ForClause> ::=")]
		public ForClause() {}
	}
}