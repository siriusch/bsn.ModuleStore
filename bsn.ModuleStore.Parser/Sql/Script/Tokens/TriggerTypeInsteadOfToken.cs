using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public class TriggerTypeInsteadOfToken: TriggerTypeToken {
		[Rule("<TriggerType> ::= ~INSTEAD ~OF")]
		public TriggerTypeInsteadOfToken() {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("INSTEAD OF");
		}
	}
}