using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public class FulltextChangeTrackingToken: SqlToken {
		[Rule("<FulltextChangeTracking> ::=")]
		public FulltextChangeTrackingToken() {}

		public virtual FulltextChangeTracking FulltextChangeTracking {
			get {
				return FulltextChangeTracking.Unspecified;
			}
		}
	}
}