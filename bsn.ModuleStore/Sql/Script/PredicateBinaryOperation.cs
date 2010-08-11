using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PredicateBinaryOperation<T>: Predicate where T: SqlComputable {
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
			Debug.Assert(left != null);
			Debug.Assert(operation != null);
			Debug.Assert(right != null);
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

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(left);
			writer.WriteScript(operation);
			writer.WriteScript(right);
		}
	}
}