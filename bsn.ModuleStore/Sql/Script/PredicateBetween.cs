using System;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PredicateBetween: PredicateNegable {
		private readonly Expression lowerBound;
		private readonly Expression upperBound;
		private readonly Expression value;

		[Rule("<PredicateBetween> ::= <Expression> BETWEEN <Expression> AND <Expression>", ConstructorParameterMapping = new[] {0, 2, 4})]
		public PredicateBetween(Expression value, Expression min, Expression max): this(value, null, min, max) {}

		[Rule("<PredicateBetween> ::= <Expression> NOT BETWEEN <Expression> AND <Expression>", ConstructorParameterMapping = new[] {0, 1, 3, 5})]
		public PredicateBetween(Expression value, IToken not, Expression lowerBound, Expression upperBound): base(not != null) {
			this.value = value;
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

		public Expression Value {
			get {
				return value;
			}
		}
	}
}