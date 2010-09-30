using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public class OptionToken: SqlScriptableToken, IOptional {
		[Rule("<ProcedureOptionGroup> ::=")]
		[Rule("<ViewOptionalAttribute> ::=")]
		[Rule("<OptionalWithTies> ::=")]
		[Rule("<OptionalFunctionOption> ::=")]
		public OptionToken() {}

		protected virtual string OptionSpecifier {
			get {
				return string.Empty;
			}
		}

		public override sealed void WriteTo(SqlWriter writer) {
			Debug.Assert(HasValue);
			writer.Write("WITH ");
			writer.Write(OptionSpecifier);
		}

		public bool HasValue {
			get {
				return GetType() != typeof(OptionToken);
			}
		}
	}
}