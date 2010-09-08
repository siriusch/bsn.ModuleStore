using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnExpressionItem: ColumnItem {
		private readonly AliasName aliasName;
		private readonly Expression expression;

		[Rule("<ColumnItem> ::= <Expression> <OptionalAlias>")]
		public ColumnExpressionItem(Expression expression, Optional<AliasName> aliasName): this(aliasName, expression) {}

		[Rule("<ColumnItem> ::= <AliasName> '=' <Expression>", ConstructorParameterMapping = new[] {0, 2})]
		public ColumnExpressionItem(AliasName aliasName, Expression expression) {
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