using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PrintStatement: SqlStatement {
		[Rule("")]
		public PrintStatement() {}
	}
}