using System;
using System.Text.RegularExpressions;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("LANGUAGE_LCID")]
	public sealed class LanguageLcid: SqlScriptableToken {
		private static readonly Regex rxParse = new Regex(@"^LANGUAGE\s+(?<value>0x(?<hex>[0-9a-f]+)|(?<int>[0-9]+)|'(?<str>([^']+|'')*)')$", RegexOptions.CultureInvariant|RegexOptions.ExplicitCapture|RegexOptions.IgnoreCase|RegexOptions.Singleline);

		private readonly string value;

		public LanguageLcid(string value) {
			Match match = rxParse.Match(value);
			if (!match.Success) {
				throw new ArgumentException("Invalid language format");
			}
			this.value = match.Groups["value"].Value;
		}

		public string Value {
			get {
				return value;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("LANGUAGE ");
			writer.Write(value);
		}
	}
}