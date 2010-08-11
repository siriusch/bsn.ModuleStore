using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PredicateBetween: PredicateNegable {
		private readonly Expression lowerBound;
		private readonly Expression upperBound;
		private readonly Expression valueExpression;

		[Rule("<PredicateBetween> ::= <Expression> BETWEEN <Expression> AND <Expression>", ConstructorParameterMapping = new[] {0, 2, 4})]
		public PredicateBetween(Expression valueExpression, Expression min, Expression max): this(valueExpression, false, min, max) {}

		protected PredicateBetween(Expression valueExpression, bool not, Expression lowerBound, Expression upperBound): base(not) {
			this.valueExpression = valueExpression;
			this.lowerBound = lowerBound;
			this.upperBound = upperBound;
		}

		public Expression LowerBound {
			get {
				return lowerBound;
			}
		}

		public Expression UpperBound {
			get {
				return upperBound;
			}
		}

		public Expression ValueExpression {
			get {
				return valueExpression;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(valueExpression);
			base.WriteTo(writer);
			writer.Write(" BETWEEN ");
			writer.WriteScript(lowerBound);
			writer.Write(" AND ");
			writer.WriteScript(upperBound);
		}
	}
}