// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PredicateBinaryOperation<T>: Predicate where T: SqlComputable {
		private readonly T left;
		private readonly OperationToken operation;
		private readonly T right;

		[Rule("<PredicateOr> ::= <PredicateAnd> OR <PredicateOr>", typeof(Predicate))]
		[Rule("<PredicateAnd> ::= <PredicateNot> AND <PredicateAnd>", typeof(Predicate))]
		[Rule("<PredicateCompare> ::= <Expression> '=' <Expression>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '>' <Expression>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '<' <Expression>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '>=' <Expression>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '<=' <Expression>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '<>' <Expression>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '!=' <Expression>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '!<' <Expression>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '!>' <Expression>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '=' <SubqueryTest>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '>' <SubqueryTest>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '<' <SubqueryTest>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '>=' <SubqueryTest>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '<=' <SubqueryTest>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '<>' <SubqueryTest>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '!=' <SubqueryTest>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '!<' <SubqueryTest>", typeof(Expression))]
		[Rule("<PredicateCompare> ::= <Expression> '!>' <SubqueryTest>", typeof(Expression))]
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
			WriteCommentsTo(writer);
			writer.WriteScript(left, WhitespacePadding.None);
			writer.WriteScript(operation, WhitespacePadding.None);
			writer.WriteScript(right, WhitespacePadding.None);
		}
	}
}
