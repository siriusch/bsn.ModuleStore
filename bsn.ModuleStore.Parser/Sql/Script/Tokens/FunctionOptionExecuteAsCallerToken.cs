using System;

using bsn.GoldParser.Semantic;

[assembly: RuleTrim("<OptionalFunctionOption> ::= WITH EXECUTE_AS_CALLER", "EXECUTE_AS_CALLER")]

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("EXECUTE_AS_CALLER")]
	public sealed class FunctionOptionExecuteAsCallerToken: FunctionOptionToken {
		public FunctionOptionExecuteAsCallerToken() {}

		public override FunctionOption FunctionOption {
			get {
				return FunctionOption.ExecuteAsCaller;
			}
		}
	}
}