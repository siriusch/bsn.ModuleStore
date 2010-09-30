using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class OptionTiesToken: OptionToken {
		[Rule("<OptionalWithTies> ::= ~WITH ~TIES")]
		public OptionTiesToken() {}

		protected override string OptionSpecifier {
			get {
				return "TIES";
			}
		}
	}
}