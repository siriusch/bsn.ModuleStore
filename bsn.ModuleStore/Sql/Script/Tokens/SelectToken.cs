using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("SELECT")]
	public sealed class SelectToken: DmlOperationToken {
		public SelectToken() {}

		public override DmlOperation Operation {
			get {
				return DmlOperation.Select;
			}
		}
	}
}