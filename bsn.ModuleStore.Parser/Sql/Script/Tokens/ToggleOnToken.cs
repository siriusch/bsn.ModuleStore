using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("ON")]
	public sealed class ToggleOnToken: ToggleToken {
		public override bool On {
			get {
				return true;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("ON");
		}
	}
}