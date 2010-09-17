using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PredicateBetween: PredicateNegable {
		private readonly Expression lowerBound;
		private readonly Expression upperBound;
		private readonly Expression valueExpression;

		[Rule("<PredicateBetween> ::= <Expression> ~BETWEEN <Expression> ~AND <Expression>")]
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
			WriteCommentsTo(writer);
			writer.WriteScript(valueExpression, WhitespacePadding.None);
			base.WriteTo(writer);
			writer.Write(" BETWEEN ");
			writer.WriteScript(lowerBound, WhitespacePadding.None);
			writer.Write(" AND ");
			writer.WriteScript(upperBound, WhitespacePadding.None);
		}
	}
}