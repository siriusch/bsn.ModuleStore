using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TransactionName: SqlName {
		[Rule("<TransactionName> ::= Id")]
		public TransactionName(Identifier name): base(name.Value) {}
	}
}