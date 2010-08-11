using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public class TriggerTypeAfterToken: TriggerTypeToken {
		[Rule("<TriggerType> ::= Id")]
		public TriggerTypeAfterToken(Identifier identifier) {
			Debug.Assert(identifier != null);
			if (!identifier.Value.Equals("AFTER", StringComparison.OrdinalIgnoreCase)) {
				throw new ArgumentException("AFTER token expected");
			}
		}

		public override TriggerType TriggerType {
			get {
				return TriggerType.After;
			}
		}
	}
}