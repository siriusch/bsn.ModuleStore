using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CursorName: SqlName {
		[Rule("<CursorName> ::= Id")]
		public CursorName(Identifier identifier): base(identifier.Value) {}
	}
}