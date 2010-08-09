using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnWildcardItem: ColumnItem {
		private readonly Expression expression;
		private readonly AliasName aliasName;

		[Rule("<ColumnItem> ::= <Expression> <OptionalAlias>")]
		public ColumnWildcardItem(Expression expression, Optional<AliasName> aliasName) {
			if (expression == null) {
				throw new ArgumentNullException("expression");
			}
			this.expression = expression;
			this.aliasName = aliasName;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public AliasName AliasName {
			get {
				return aliasName;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteScript(expression);
			if (aliasName != null) {
				writer.Write(" AS ");
				writer.WriteScript(aliasName);
			}
		}
	}
}