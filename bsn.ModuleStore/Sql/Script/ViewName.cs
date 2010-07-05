using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ViewName: SqlName {
		[Rule("<ViewName> ::= Id")]
		public ViewName(Identifier identifier): base(identifier.Value) {}
	}
}