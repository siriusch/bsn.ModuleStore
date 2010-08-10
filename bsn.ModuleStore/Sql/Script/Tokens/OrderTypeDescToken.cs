using System;
using System.Data.SqlClient;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class OrderTypeDescToken: OrderTypeToken {
		[Rule("<OrderType> ::= DESC")]
		public OrderTypeDescToken() {}

		public override SortOrder Order {
			get {
				return SortOrder.Descending;
			}
		}
	}
}