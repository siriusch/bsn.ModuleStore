using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("QuotedId")]
	public sealed class QuotedIdentifier: Identifier {
		internal static bool TryDequote(string value, out string result) {
			if ((value != null) && (value.Length > 2)) {
				switch (value[0]) {
				case '[':
					if (value.IndexOf(']') == value.Length-1) {
						result = value.Substring(1, value.Length-2);
						return true;
					}
					break;
				case '"':
					if (value[value.Length-1] == '"') {
						result = value.Substring(1, value.Length-2).Replace(@"""""", @"""");
						return true;
					}
					break;
				}
			}
			result = null;
			return false;
		}

		private readonly string value;

		public QuotedIdentifier(string id): base(id) {
			if (!TryDequote(id, out value)) {
				throw new ArgumentException("Malformed identifier", "id");
			}
		}

		public override string Value {
			get {
				return value;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteDelimitedIdentifier(value);
		}
	}
}