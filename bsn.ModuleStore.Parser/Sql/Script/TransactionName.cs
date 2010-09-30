using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TransactionName: SqlQuotedName {
		[Rule("<TransactionName> ::= Id")]
		[Rule("<TransactionName> ::= QuotedId")]
		public TransactionName(Identifier name): base(name.Value) {}
	}
}