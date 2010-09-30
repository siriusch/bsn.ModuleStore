using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class FulltextChangeTrackingAuto: FulltextChangeTracking {
		[Rule("<FulltextChangeTracking> ::= ~WITH ~CHANGE_TRACKING ~AUTO")]
		public FulltextChangeTrackingAuto() {}

		public override FulltextChangeTrackingKind ChangeTracking {
			get {
				return FulltextChangeTrackingKind.Auto;
			}
		}

		protected override string ChangeTrackingSpecifier {
			get {
				return "AUTO";
			}
		}
	}
}