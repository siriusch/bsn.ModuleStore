using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("OFF")]
	public sealed class ToggleOffToken: ToggleToken {
		public override bool On {
			get {
				return false;
			}
		}
	}
}