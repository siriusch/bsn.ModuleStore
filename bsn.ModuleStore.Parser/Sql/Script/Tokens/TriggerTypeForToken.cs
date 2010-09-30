using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public class TriggerTypeForToken: TriggerTypeToken {
		[Rule("<TriggerType> ::= ~FOR")]
		public TriggerTypeForToken() {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("FOR");
		}
	}
}