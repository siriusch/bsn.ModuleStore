using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class IndexName: SqlName {
		[Rule("<IndexName> ::= Id")]
		public IndexName(Identifier identifier): base(identifier.Value) {}
	}
}