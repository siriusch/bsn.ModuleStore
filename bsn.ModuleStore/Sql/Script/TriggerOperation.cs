using System;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TriggerOperation: SqlToken {
		private readonly IToken token;

		[Rule("<TriggerOperation> ::= INSERT")]
		[Rule("<TriggerOperation> ::= UPDATE")]
		[Rule("<TriggerOperation> ::= DELETE")]
		public TriggerOperation(IToken token) {
			if (token == null) {
				throw new ArgumentNullException("token");
			}
			this.token = token;
		}
	}
}