using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ViewName: SqlQuotedName {
		[Rule("<ViewName> ::= Id")]
		public ViewName(Identifier identifier): base(identifier.Value) {}
	}
}