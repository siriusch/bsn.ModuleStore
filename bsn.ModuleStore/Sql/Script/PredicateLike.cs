using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PredicateLike: PredicateNegable {
		private readonly StringLiteral escape;
		private readonly StringLiteral text;
		private readonly Expression valueExpression;

		[Rule("<PredicateLike> ::= <Expression> LIKE <CollableStringLiteral>", ConstructorParameterMapping = new[] {0, 2})]
		public PredicateLike(Expression valueExpression, StringLiteral text): this(valueExpression, false, text, null) {}

		[Rule("<PredicateLike> ::= <Expression> LIKE <CollableStringLiteral> ESCAPE StringLiteral", ConstructorParameterMapping = new[] {0, 2, 4})]
		public PredicateLike(Expression valueExpression, StringLiteral text, StringLiteral escape): this(valueExpression, false, text, escape) {}

		protected PredicateLike(Expression valueExpression, bool not, StringLiteral text, StringLiteral escape): base(not) {
			Debug.Assert(valueExpression != null);
			Debug.Assert(text != null);
			this.valueExpression = valueExpression;
			this.text = text;
			this.escape = escape;
		}

		public StringLiteral Escape {
			get {
				return escape;
			}
		}

		public StringLiteral Text {
			get {
				return text;
			}
		}

		public Expression ValueExpression {
			get {
				return valueExpression;
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