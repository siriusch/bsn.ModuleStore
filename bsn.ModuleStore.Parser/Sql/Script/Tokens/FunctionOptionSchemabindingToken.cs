using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class FunctionOptionSchemabindingToken: OptionToken {
		[Rule("<OptionalFunctionOption> ::= ~WITH ~SCHEMABINDING")]
		public FunctionOptionSchemabindingToken() {}

		protected override string OptionSpecifier {
			get {
				return "SCHEMABINDING";
			}
		}
	}
}