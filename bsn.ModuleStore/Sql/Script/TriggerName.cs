using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TriggerName: SqlName {
		[Rule("<TriggerName> ::= Id")]
		public TriggerName(Identifier identifier): base(identifier.Value) {}
	}
}