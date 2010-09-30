using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class OptionRecompileToken: OptionToken {
		[Rule("<ProcedureOptionGroup> ::= ~WITH ~RECOMPILE")]
		public OptionRecompileToken() {}

		protected override string OptionSpecifier {
			get {
				return "RECOMPILE";
			}
		}
	}
}