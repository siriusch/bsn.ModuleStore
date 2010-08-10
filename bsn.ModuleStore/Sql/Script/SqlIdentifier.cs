using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlIdentifier: SqlToken {
		private static readonly Regex rxDequote = new Regex(@"(?<=^\[)[^\]]+(?=\]$)|(?<=^"")[^""]+(?=""$)|^[^""\[\]\s]+$");

		internal static bool TryDequote(string id, out string value) {
			Match match = rxDequote.Match(id);
			if (match.Success) {
				value = id.Length == match.Length ? id : match.Value;
				return true;
			}
			value = null;
			return false;
		}

		private readonly string original;
		private readonly string value;

		protected SqlIdentifier(string id) {
			if (id == null) {
				throw new ArgumentNullException("id");
			}
			original = id;
			if (!TryDequote(id, out value)) {
				throw new ArgumentException("Malformed identifier", "id");
			}
		}

		public string Value {
			get {
				return value;
			}
		}

		protected string Original {
			get {
				return original;
			}
		}
	}
}