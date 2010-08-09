using System;
using System.IO;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PredicateLike: PredicateNegable {
		private readonly Expression valueExpression;
		private readonly StringLiteral text;
		private readonly StringLiteral escape;

		[Rule("<PredicateLike> ::= <Expression> LIKE <CollableStringLiteral>", ConstructorParameterMapping = new[] {0, 2})]
		public PredicateLike(Expression valueExpression, StringLiteral text): this(valueExpression, false, text, null) {}

		[Rule("<PredicateLike> ::= <Expression> LIKE <CollableStringLiteral> ESCAPE StringLiteral", ConstructorParameterMapping=new[] { 0, 2, 4 })]
		public PredicateLike(Expression valueExpression, StringLiteral text, StringLiteral escape): this(valueExpression, false, text, escape) {}

		protected PredicateLike(Expression valueExpression, bool not, StringLiteral text, StringLiteral escape): base(not) {
			if (valueExpression == null) {
				throw new ArgumentNullException("valueExpression");
			}
			if (text == null) {
				throw new ArgumentNullException("text");
			}
			this.valueExpression = valueExpression;
			this.text = text;
			this.escape = escape;
		}

		public Expression ValueExpression {
			get {
				return valueExpression;
			}
		}

		public StringLiteral Text {
			get {
				return text;
			}
		}

		public StringLiteral Escape {
			get {
				return escape;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteScript(valueExpression);
			base.WriteTo(writer);
			writer.Write(" LIKE ");
			writer.WriteScript(text);
			if (escape != null) {
				writer.Write(" ESCAPE ");
				writer.WriteScript(escape);
			}
		}
	}
}