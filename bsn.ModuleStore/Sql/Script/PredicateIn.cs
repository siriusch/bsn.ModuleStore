using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PredicateIn: PredicateNegable {
		private readonly Tuple tuple;
		private readonly Expression valueExpression;

		[Rule("<PredicateIn> ::= <Expression> IN <Tuple>", ConstructorParameterMapping = new[] {0, 2})]
		public PredicateIn(Expression valueExpression, Tuple tuple): this(valueExpression, false, tuple) {}

		protected PredicateIn(Expression valueExpression, bool not, Tuple tuple): base(not) {
			if (valueExpression == null) {
				throw new ArgumentNullException("valueExpression");
			}
			if (tuple == null) {
				throw new ArgumentNullException("tuple");
			}
			this.valueExpression = valueExpression;
			this.tuple = tuple;
		}

		public Tuple Tuple {
			get {
				return tuple;
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
			writer.Write(" IN ");
			writer.WriteScript(tuple);
		}
	}
}