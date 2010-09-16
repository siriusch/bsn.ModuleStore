using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class FulltextChangeTrackingCustomToken: FulltextChangeTrackingToken {
		private readonly FulltextChangeTracking mode;

		[Rule("<FulltextChangeTracking> ::= ~WITH_CHANGE_TRACKING Id")]
		public FulltextChangeTrackingCustomToken(Identifier identifier) {
			if (string.Equals(identifier.Value, "MANUAL", StringComparison.OrdinalIgnoreCase)) {
				mode = FulltextChangeTracking.Manual;
			} else if (string.Equals(identifier.Value, "AUTO", StringComparison.OrdinalIgnoreCase)) {
				mode = FulltextChangeTracking.Auto;
			}
		}

		public override FulltextChangeTracking FulltextChangeTracking {
			get {
				return mode;
			}
		}
	}
}