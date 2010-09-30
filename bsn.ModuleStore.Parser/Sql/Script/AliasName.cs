using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class AliasName: SqlQuotedName {
		[Rule("<AliasName> ::= Id")]
		[Rule("<AliasName> ::= QuotedId")]
		public AliasName(Identifier identifier): base(identifier.Value) {}
	}
}