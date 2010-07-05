using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TableName: SqlName {
		[Rule("<TableName> ::= Id")]
		[Rule("<TableName> ::= TempTableId")]
		public TableName(SqlIdentifier identifier): base(identifier.Value) {}
	}
}