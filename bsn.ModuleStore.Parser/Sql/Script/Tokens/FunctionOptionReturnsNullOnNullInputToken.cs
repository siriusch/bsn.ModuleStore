using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class FunctionOptionReturnsNullOnNullInputToken: OptionToken {
		[Rule("<OptionalFunctionOption> ::= ~WITH ~RETURNS ~NULL ~ON ~NULL ~INPUT")]
		public FunctionOptionReturnsNullOnNullInputToken() {}

		protected override string OptionSpecifier {
			get {
				return "RETURNS NULL ON NULL INPUT";
			}
		}
	}
}