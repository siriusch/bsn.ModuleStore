using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class OrderExpression: SqlToken, IScriptable {
		private readonly Expression expression;
		private readonly SortOrder oderType;

		[Rule("<Order> ::= <Expression> <OrderType>")]
		public OrderExpression(Expression expression, OrderTypeToken oderType) {
			Debug.Assert(expression != null);
			this.expression = expression;
			this.oderType = oderType.Order;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public SortOrder OderType {
			get {
				return oderType;
			}
		}

		public void WriteTo(SqlWriter writer) {
			writer.WriteScript(expression);
			writer.WriteValue(oderType, " ", null);
		}
	}
}