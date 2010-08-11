using System;
using System.Data.SqlClient;
using System.IO;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class IndexColumn: SqlToken, IScriptable {
		private readonly ColumnName columnName;
		private readonly SortOrder order;

		[Rule("<IndexColumn> ::= <ColumnName> <OrderType>")]
		public IndexColumn(ColumnName columnName, OrderTypeToken order) {
			this.columnName = columnName;
			this.order = order.Order;
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}

		public SortOrder Order {
			get {
				return order;
			}
		}

		public void WriteTo(SqlWriter writer) {
			writer.WriteScript(columnName);
			writer.WriteValue(order, " ", null);
		}
	}
}