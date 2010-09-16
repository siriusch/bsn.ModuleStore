using System;

using bsn.GoldParser.Semantic;

[assembly:RuleTrim("<OptionalFunctionOption> ::= WITH CALLED_ON_NULL_INPUT", "CALLED_ON_NULL_INPUT")]

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("CALLED_ON_NULL_INPUT")]
	public sealed class FunctionOptionCalledOnNullInputToken: FunctionOptionToken {
		public FunctionOptionCalledOnNullInputToken() {}

		public override FunctionOption FunctionOption {
			get {
				return FunctionOption.CalledOnNullInput;
			}
		}
	}
}