using System;
using System.Data.SqlClient;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("DESC")]
	public sealed class OrderTypeDescToken: OrderTypeToken {
		public override SortOrder Order {
			get {
				return SortOrder.Descending;
			}
		}
	}
}