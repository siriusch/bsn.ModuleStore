using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TriggerName: SqlQuotedName {
		[Rule("<TriggerName> ::= Id")]
		public TriggerName(Identifier identifier): base(identifier.Value) {}
	}
}