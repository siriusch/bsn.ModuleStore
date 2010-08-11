using System;
using System.IO;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TopExpression: SqlToken, IScriptable, IOptional {
		private readonly Expression expression;
		private readonly bool percent;
		private readonly bool withTies;

		[Rule("<OptionalTop> ::=")]
		public TopExpression(): this(null, null, null) {}

		[Rule("<TopLegacy> ::= TOP IntegerLiteral <OptionalPercent>", ConstructorParameterMapping = new[] {1, 2})]
		public TopExpression(IntegerLiteral integerLiteral, Optional<PercentToken> percent): this(integerLiteral, percent, null) {}

		[Rule("<Top> ::= TOP '(' <Expression> ')' <OptionalPercent> <OptionalWithTies>", ConstructorParameterMapping = new[] {2, 4, 5})]
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

		public bool HasValue {
			get {
				return expression != null;
			}
		}

		public void WriteTo(TextWriter writer) {
			if (HasValue) {
				writer.Write("TOP (");
				writer.WriteScript(expression);
				writer.Write(')');
				writer.WritePercent(percent, " ", null);
				writer.WriteWithTies(withTies, " ", null);
			}
		}
	}
}