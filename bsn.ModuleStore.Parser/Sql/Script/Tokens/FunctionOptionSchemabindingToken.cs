using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class FunctionOptionSchemabindingToken: FunctionOptionToken {
		[Rule("<OptionalFunctionOption> ::= ~WITH_SCHEMABINDING")]
		public FunctionOptionSchemabindingToken() {}

		public override FunctionOption FunctionOption {
			get {
				return FunctionOption.Schemabinding;
			}
		}
	}
}