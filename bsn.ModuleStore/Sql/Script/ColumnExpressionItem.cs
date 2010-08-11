using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnExpressionItem: ColumnItem {
		private readonly AliasName aliasName;
		private readonly Expression expression;

		[Rule("<ColumnItem> ::= <Expression> <OptionalAlias>")]
		public ColumnExpressionItem(Expression expression, Optional<AliasName> aliasName): this(aliasName, expression) {}

		[Rule("<ColumnItem> ::= <AliasName> '=' <Expression>", ConstructorParameterMapping = new[] {0, 2})]
		public ColumnExpressionItem(AliasName aliasName, Expression expression) {
			if (expression == null) {
				throw new ArgumentNullException("expression");
			}
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

		public override void WriteTo(TextWriter writer) {
			writer.WriteScript(expression);
			writer.WriteScript(aliasName, " AS ", null);
		}
	}
}