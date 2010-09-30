using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class FunctionOptionExecuteAsCallerToken: OptionToken {
		[Rule("<OptionalFunctionOption> ::= ~WITH ~EXECUTE ~AS ~CALLER")]
		public FunctionOptionExecuteAsCallerToken() {}

		protected override string OptionSpecifier {
			get {
				return "EXECUTE AS CALLER";
			}
		}
	}
}