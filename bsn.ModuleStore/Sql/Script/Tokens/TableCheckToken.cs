using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public class TableCheckToken: SqlToken {
		[Rule("<TableCheck> ::=")]
		public TableCheckToken() {}

		public virtual TableCheck TableCheck {
			get {
				return TableCheck.Unspecified;
			}
		}
	}
}