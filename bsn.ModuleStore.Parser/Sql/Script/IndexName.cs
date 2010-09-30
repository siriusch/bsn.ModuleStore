using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class IndexName: SqlQuotedName {
		[Rule("<IndexName> ::= Id")]
		[Rule("<IndexName> ::= QuotedId")]
		public IndexName(Identifier identifier): base(identifier.Value) {}
	}
}