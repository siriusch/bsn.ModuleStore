using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class FulltextChangeTrackingManual: FulltextChangeTracking {
		[Rule("<FulltextChangeTracking> ::= ~WITH ~CHANGE_TRACKING ~MANUAL")]
		public FulltextChangeTrackingManual() {}

		public override FulltextChangeTrackingKind ChangeTracking {
			get {
				return FulltextChangeTrackingKind.Manual;
			}
		}

		protected override string ChangeTrackingSpecifier {
			get {
				return "MANUAL";
			}
		}
	}
}