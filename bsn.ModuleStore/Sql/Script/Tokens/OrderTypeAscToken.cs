using System.Data.SqlClient;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class OrderTypeAscToken: OrderTypeToken {
		[Rule("<OrderType> ::= ASC")]
		public OrderTypeAscToken() {
		}

		public override SortOrder Order {
			get {
				return SortOrder.Ascending;
			}
		}
	}
}