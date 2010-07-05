using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("ON")]
	[Terminal("OFF")]
	public class Toggle: SqlToken {
		public static implicit operator bool(Toggle toggle) {
			return toggle.On;
		}

		private readonly bool on;

		public Toggle(string value) {
			on = string.Equals("ON", value, StringComparison.OrdinalIgnoreCase);
		}

		public bool On {
			get {
				return on;
			}
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write(on ? "ON" : "OFF");
		}
	}
}