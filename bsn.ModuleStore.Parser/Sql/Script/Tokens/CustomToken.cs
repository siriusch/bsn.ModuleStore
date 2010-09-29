using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("READ_COMMITTED")]
	[Terminal("READ_UNCOMMITTED")]
	[Terminal("REPEATABLE_READ")]
	public class CustomToken: SqlScriptableToken {
		private static readonly Regex rxSpaces = new Regex(@"\s+");
		private readonly string value;

		public CustomToken(string value) {
			this.value = rxSpaces.Replace(value, " ");
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write(value);
		}
	}
}
