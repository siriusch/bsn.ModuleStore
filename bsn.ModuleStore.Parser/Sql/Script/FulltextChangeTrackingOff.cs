using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class FulltextChangeTrackingOff: FulltextChangeTracking {
		[Rule("<FulltextChangeTracking> ::= ~WITH ~CHANGE_TRACKING ~OFF")]
		public FulltextChangeTrackingOff() {}

		public override FulltextChangeTrackingKind ChangeTracking {
			get {
				return FulltextChangeTrackingKind.Off;
			}
		}

		protected override string ChangeTrackingSpecifier {
			get {
				return "OFF";
			}
		}
	}
}