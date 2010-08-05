using System;
using System.Data.SqlClient;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class OrderType: SqlToken {
		private readonly SortOrder order;

		[Rule("<OrderType> ::=")]
		public OrderType(): this(null) {}

		[Rule("<OrderType> ::= ASC")]
		[Rule("<OrderType> ::= DESC")]
		public OrderType(IToken token) {
			if (token == null) {
				order = SortOrder.Unspecified;
			} else {
				order = (token.Symbol.Name.Equals("DESC", StringComparison.Ordinal)) ? SortOrder.Descending : SortOrder.Ascending;
			}
		}

		public SortOrder Order {
			get {
				return order;
			}
		}
	}
}