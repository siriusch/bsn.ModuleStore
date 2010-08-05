using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class DestinationRowset: SqlToken {
		[Rule("<DestinationRowset> ::= <VariableName>")]
		[Rule("<DestinationRowset> ::= <TableName>")]
		public DestinationRowset(SqlName name) {}
	}
}