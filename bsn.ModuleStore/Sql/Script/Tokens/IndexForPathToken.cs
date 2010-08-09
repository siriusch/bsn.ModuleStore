using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("FOR_PATH")]
	public sealed class IndexForPathToken: IndexForToken {
		public IndexForPathToken() {
		}

		public override IndexFor IndexFor {
			get {
				return IndexFor.Path;
			}
		}
	}
}