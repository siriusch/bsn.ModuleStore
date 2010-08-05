using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PredicateBinaryOperation<T>: Predicate where T: SqlComputable {
		private readonly T left;
		private readonly OperationToken operation;
		private readonly T right;

		[Rule("<PredicateOr> ::= <PredicateAnd> OR <PredicateOr>", typeof(Predicate))]
		[Rule("<PredicateAnd> ::= <PredicateNot> AND <PredicateAnd>", typeof(Predicate))]
		[Rule("<PredicateCompare> ::= <Expression> '=' <Expression>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '<>' <Expression>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '>' <Expression>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '>=' <Expression>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '<' <Expression>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '<=' <Expression>", typeof(Expression))]
		public PredicateBinaryOperation(T left, OperationToken operation, T right) {
			if (left == null) {
				throw new ArgumentNullException("left");
			}
			if (operation == null) {
				throw new ArgumentNullException("operation");
			}
			if (right == null) {
				throw new ArgumentNullException("right");
			}
			this.left = left;
			this.operation = operation;
			this.right = right;
		}

		public T Left {
			get {
				return left;
			}
		}

		public OperationToken Operation {
			get {
				return operation;
			}
		}

		public T Right {
			get {
				return right;
			}
		}
	}
}