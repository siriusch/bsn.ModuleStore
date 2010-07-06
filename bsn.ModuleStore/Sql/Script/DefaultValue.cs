using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class DefaultValue: SqlToken {
		private readonly Literal literal;

		[Rule("<OptionalDefault> ::=")]
		public DefaultValue(): this(null) {}

		[Rule("<OptionalDefault> ::= '=' <Literal>", ConstructorParameterMapping = new[] {1})]
		public DefaultValue(Literal literal) {
			this.literal = literal;
		}

		public Literal Literal {
			get {
				return literal;
			}
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			if (literal != null) {
				writer.Write("= ");
				literal.WriteTo(writer);
			}
		}
	}
}