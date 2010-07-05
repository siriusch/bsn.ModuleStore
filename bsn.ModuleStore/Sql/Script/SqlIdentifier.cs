using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlIdentifier: SqlToken {
		private static readonly Regex rxDequote = new Regex(@"(?<=^\[)[^\]]+(?=\]$)|(?<=^"")[^""]+(?=""$)|^[^""\[\]]+$");

		private readonly string value;

		protected SqlIdentifier(string id) {
			if (id == null) {
				throw new ArgumentNullException("id");
			}
			Match match = rxDequote.Match(id);
			if (!match.Success) {
				throw new ArgumentException("Malformed identifier", "id");
			}
			value = match.Value;
		}

		public string Value {
			get {
				return value;
			}
		}
	}
}