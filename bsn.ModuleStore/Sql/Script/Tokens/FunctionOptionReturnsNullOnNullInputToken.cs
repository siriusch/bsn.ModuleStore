using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class FunctionOptionReturnsNullOnNullInputToken: FunctionOptionToken {
		[Rule("<OptionalFunctionOption> ::= WITH RETURNS_NULL_ON_NULL_INPUT", AllowTruncationForConstructor = true)]
		public FunctionOptionReturnsNullOnNullInputToken() {}

		public override FunctionOption FunctionOption {
			get {
				return FunctionOption.ReturnsNullOnNullInput;
			}
		}
	}
}