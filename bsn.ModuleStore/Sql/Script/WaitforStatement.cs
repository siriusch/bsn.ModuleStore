using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class WaitforStatement: SqlStatement {
		[Rule("")]
		public WaitforStatement() {}
	}
}