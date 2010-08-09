using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("UPDATE")]
	public sealed class UpdateToken: DmlOperationToken {
		public UpdateToken() {
		}

		public override DmlOperation Operation {
			get {
				return DmlOperation.Update;
			}
		}
	}
}