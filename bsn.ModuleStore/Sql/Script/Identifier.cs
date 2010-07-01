using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("Id")]
	[Terminal("LocalId")]
	[Terminal("SystemVarId")]
	[Terminal("SystemFuncId")]
	[Terminal("TempTableId")]
	public class Identifier: SqlToken {
		private static readonly Regex rxDequote = new Regex(@"(?<=^\[)[^\]]+(?=\]$)|(?<=^"")[^""]+(?=""$)|^[^""\[\]]+$");

		private readonly string name;

		public Identifier(string id) {
			if (id == null) {
				throw new ArgumentNullException("id");
			}
			Match match = rxDequote.Match(id);
			if (!match.Success) {
				throw new ArgumentException("Malformed identifier", "id");
			}
			this.name = match.Value;
		}

		public string Name {
			get {
				return name;
			}
		}
	}
}
