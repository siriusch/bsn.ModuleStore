using System;
using System.Diagnostics;
using System.Text;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("StringLiteral")]
	public sealed class StringLiteral: Literal<string> {
		internal static bool ParseIsUnicode(string value) {
			return value.StartsWith("N", StringComparison.OrdinalIgnoreCase);
		}

		internal static string ParseValue(string value) {
			StringBuilder result = new StringBuilder(value.Length-2);
			int i = 0;
			if (value[i] != '\'') {
				i++;
			}
			Debug.Assert(value[i] == '\'');
			i++;
			bool keepQuote = false;
			while (i < value.Length) {
				char c = value[i++];
				bool isNotQuote = c != '\'';
				if (isNotQuote || keepQuote) {
					result.Append(c);
					keepQuote = false;
				} else {
					keepQuote = true;
				}
			}
			return result.ToString();
		}

		private readonly CollationName collation;
		private readonly bool isUnicode;

		[Rule("<CollableStringLiteral> ::= StringLiteral ~COLLATE <CollationName>")]
		public StringLiteral(StringLiteral value, CollationName collation): this(value.Value, value.IsUnicode, collation) {}

		public StringLiteral(string value): this(ParseValue(value), ParseIsUnicode(value), null) {}

		private StringLiteral(string parsedValue, bool isUnicode, CollationName collation): base(parsedValue) {
			this.isUnicode = isUnicode;
			this.collation = collation;
		}

		public CollationName Collation {
			get {
				return collation;
			}
		}

		public bool IsUnicode {
			get {
				return isUnicode;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write('\'');
			writer.Write(Value.Replace("'", "''"));
			writer.Write('\'');
			writer.WriteScript(collation, WhitespacePadding.SpaceBefore);
		}
	}
}