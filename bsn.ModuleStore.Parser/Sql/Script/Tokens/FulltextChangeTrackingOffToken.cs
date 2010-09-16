using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class FulltextChangeTrackingOffToken: FulltextChangeTrackingToken {
		[Rule("<FulltextChangeTracking> ::= ~WITH_CHANGE_TRACKING ~OFF")]
		public FulltextChangeTrackingOffToken() {}

		public override FulltextChangeTracking FulltextChangeTracking {
			get {
				return FulltextChangeTracking.Off;
			}
		}
	}
}