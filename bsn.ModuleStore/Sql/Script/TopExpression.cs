using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TopExpression: SqlToken {
		private readonly Expression expression;
		private readonly bool percent;
		private readonly bool withTies;

		[Rule("<Top> ::=")]
		public TopExpression(): this(null, null, null) {}

		[Rule("<TopLegacy> ::= TOP IntegerLiteral <OptionalPercent>", ConstructorParameterMapping = new[] {1, 2})]
		public TopExpression(IntegerLiteral integerLiteral, Optional<Percent> percent): this(new Expression(integerLiteral), percent, null) {}

		[Rule("<Top> ::= TOP '(' <Expression> ')' <OptionalPercent> <OptionalWithTies>", ConstructorParameterMapping = new[] {2, 4, 5})]
		public TopExpression(Expression expression, Optional<Percent> percent, Optional<WithTies> withTies) {
			this.expression = expression;
			this.percent = (percent != null) && percent.HasValue;
			this.withTies = (withTies != null) && withTies.HasValue;
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
	}
}