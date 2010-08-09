using System;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PredicateIn: PredicateNegable {
		private readonly Expression valueExpression;
		private readonly Tuple tuple;

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

		public Expression ValueExpression {
			get {
				return valueExpression;
			}
		}

		public Tuple Tuple {
			get {
				return tuple;
			}
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.WriteScript(valueExpression);
			base.WriteTo(writer);
			writer.Write(" IN ");
			writer.WriteScript(tuple);
		}
	}
}