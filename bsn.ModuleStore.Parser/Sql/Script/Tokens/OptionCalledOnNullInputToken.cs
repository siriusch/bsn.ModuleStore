using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class OptionCalledOnNullInputToken: OptionToken {
		[Rule("<OptionalFunctionOption> ::= ~WITH ~CALLED ~ON ~NULL ~INPUT")]
		public OptionCalledOnNullInputToken() {}

		protected override string OptionSpecifier {
			get {
				return "CALLED ON NULL INPUT";
			}
		}
	}
}