using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TableName: SqlQuotedName {
		[Rule("<TableName> ::= Id")]
		[Rule("<TableName> ::= QuotedId")]
		[Rule("<TableName> ::= TempTableId")]
		public TableName(SqlIdentifier identifier): this(identifier.Value) {}

		internal TableName(string name): base(name) {}
	}
}