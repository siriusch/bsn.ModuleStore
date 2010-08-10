using System;
using System.Data.SqlClient;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("ASC")]
	public sealed class OrderTypeAscToken: OrderTypeToken {
		public OrderTypeAscToken() {}

		public override SortOrder Order {
			get {
				return SortOrder.Ascending;
			}
		}
	}
}