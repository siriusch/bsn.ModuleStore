using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class FunctionOptionCalledOnNullInputToken: FunctionOptionToken {
		[Rule("<OptionalFunctionOption> ::= WITH CALLED_ON_NULL_INPUT", AllowTruncationForConstructor = true)]
		public FunctionOptionCalledOnNullInputToken() {}

		public override FunctionOption FunctionOption {
			get {
				return FunctionOption.CalledOnNullInput;
			}
		}
	}
}