using System.Data.SqlClient;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public class OrderTypeToken: SqlToken {
		[Rule("<OrderType> ::=")]
		public OrderTypeToken(): base() {}

		public virtual SortOrder Order {
			get {
				return SortOrder.Unspecified;
			}
		}
	}
}