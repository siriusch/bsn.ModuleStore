using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnExpressionItem: ColumnItem {
		private readonly AliasName aliasName;
		private readonly Expression expression;

		[Rule("<ColumnItem> ::= <Expression>")]
		public ColumnExpressionItem(Expression expression): this(expression, null) {}

		[Rule("<ColumnItem> ::= <Expression> ~<OptionalAs> <AliasName>")]
		[Rule("<ColumnItem> ::= 1:<AliasName> ~'=' 0:<Expression>")]
		public ColumnExpressionItem(Expression expression, AliasName aliasName) {
			Debug.Assert(expression != null);
			this.expression = expression;
			this.aliasName = aliasName;
		}

		public AliasName AliasName {
			get {
				return aliasName;
			}
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(expression, WhitespacePadding.None);
			writer.WriteScript(aliasName, WhitespacePadding.SpaceBefore, "AS ", null);
		}
	}
}