using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class FulltextChangeTrackingOffNoPopulationToken: FulltextChangeTrackingToken {
		[Rule("<FulltextChangeTracking> ::= WITH_CHANGE_TRACKING OFF ',' NO_POPULATION", AllowTruncationForConstructor = true)]
		public FulltextChangeTrackingOffNoPopulationToken() {
		}

		public override FulltextChangeTracking FulltextChangeTracking {
			get {
				return FulltextChangeTracking.OffNoPopulation;
			}
		}
	}
}