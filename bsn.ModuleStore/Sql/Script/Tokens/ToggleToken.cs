using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("ON")]
	[Terminal("OFF")]
	public class ToggleToken: SqlToken {
		private readonly bool on;

		public ToggleToken(string value) {
			on = string.Equals("ON", value, StringComparison.OrdinalIgnoreCase);
		}

		public bool On {
			get {
				return on;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write(on ? "ON" : "OFF");
		}
	}
}