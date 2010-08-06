using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public class TriggerTypeForToken: TriggerTypeToken {
		[Rule("<TriggerType> ::= FOR", AllowTruncationForConstructor = true)]
		public TriggerTypeForToken() {}

		public override TriggerType TriggerType {
			get {
				return TriggerType.For;
			}
		}
	}
}