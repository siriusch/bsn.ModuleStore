using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public class TriggerTypeAfterToken: TriggerTypeToken {
		[Rule("<TriggerType> ::= ~AFTER")]
		public TriggerTypeAfterToken() {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("AFTER");
		}
	}
}