using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class FunctionOptionToken: SqlToken {
		[Rule("<FunctionOption> ::=")]
		public FunctionOptionToken() {}

		public virtual FunctionOption FunctionOption {
			get {
				return FunctionOption.None;
			}
		}
	}
}