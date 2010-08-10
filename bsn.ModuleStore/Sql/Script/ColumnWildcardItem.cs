using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnWildcardItem: ColumnItem {
		private readonly AliasName aliasName;
		private readonly Expression expression;

		[Rule("<ColumnItem> ::= <Expression> <OptionalAlias>")]
		public ColumnWildcardItem(Expression expression, Optional<AliasName> aliasName) {
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