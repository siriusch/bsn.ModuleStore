using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class FulltextChangeTrackingOffNoPopulation: FulltextChangeTracking {
		[Rule("<FulltextChangeTracking> ::= ~WITH ~CHANGE_TRACKING ~OFF ~',' ~NO ~POPULATION")]
		public FulltextChangeTrackingOffNoPopulation() {}

		public override FulltextChangeTrackingKind ChangeTracking {
			get {
				return FulltextChangeTrackingKind.OffNoPopulation;
			}
		}

		protected override string ChangeTrackingSpecifier {
			get {
				return "OFF, NO POPULATION";
			}
		}
	}
}