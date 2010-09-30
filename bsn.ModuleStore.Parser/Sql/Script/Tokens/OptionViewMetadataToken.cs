using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class OptionViewMetadataToken: OptionToken {
		[Rule("<ViewOptionalAttribute> ::= ~WITH ~VIEW_METADATA")]
		public OptionViewMetadataToken() {}

		protected override string OptionSpecifier {
			get {
				return "VIEW_METADATA";
			}
		}
	}
}