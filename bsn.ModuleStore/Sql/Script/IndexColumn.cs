using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class IndexColumn: SqlToken {
		[Rule("<IndexColumn> ::= <ColumnName> <OrderType>")]
		public IndexColumn(ColumnName columnName, OrderType order) {}
	}
}