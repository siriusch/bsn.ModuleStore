using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TopExpression: SqlScriptableToken, IOptional {
		private readonly Expression expression;
		private readonly bool percent;
		private readonly bool withTies;

		[Rule("<OptionalTop> ::=")]
		public TopExpression(): this(null, null, null) {}

		[Rule("<TopLegacy> ::= ~TOP IntegerLiteral <OptionalPercent>")]
		public TopExpression(IntegerLiteral integerLiteral, Optional<PercentToken> percent): this(integerLiteral, percent, null) {}

		[Rule("<Top> ::= ~TOP ~'(' <Expression> ~')' <OptionalPercent> <OptionalWithTies>")]
		public TopExpression(Expression expression, Optional<PercentToken> percent, Optional<WithTiesToken> withTies) {
			this.expression = expression;
			this.percent = percent.HasValue();
			this.withTies = withTies.HasValue();
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public bool Percent {
			get {
				return percent;
			}
		}

		public bool WithTies {
			get {
				return withTies;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			if (HasValue) {
				writer.Write("TOP (");
				writer.WriteScript(expression, WhitespacePadding.None);
				writer.Write(')');
				if (percent) {
					writer.Write(" PERCENT");
				}
				if (withTies) {
					writer.Write(" WITH TIES");
				}
			}
		}

		public bool HasValue {
			get {
				return expression != null;
			}
		}
	}
}