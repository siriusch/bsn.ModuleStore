using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class Sequence<T>: SqlToken, IEnumerable<T> where T: SqlToken {
		private readonly T item;
		private readonly Sequence<T> next;

		[Rule("<CursorOptionList> ::=", typeof(SqlIdentifier))]
		public Sequence(): this(null, null) {}

		[Rule("<ColumnNameList> ::= <ColumnName>", typeof(SqlName))]
		[Rule("<StatementList> ::= <StatementGroup>", typeof(StatementGroup))]
		[Rule("<StatementList> ::= <StatementGroup> <Terminator>", typeof(StatementGroup), AllowTruncationForConstructor = true)]
		[Rule("<OpenxmlColumnList> ::= <OpenxmlColumn>", typeof(OpenxmlColumn))]
		public Sequence(T item): this(item, null) {}

		[Rule("<ColumnNameList> ::= <ColumnNameList> ',' <ColumnName>", typeof(SqlName), ConstructorParameterMapping = new[] {2, 0})]
		[Rule("<StatementList> ::= <StatementGroup> <Terminator> <StatementList>", typeof(StatementGroup), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<CursorOptionList> ::= Id <CursorOptionList>", typeof(SqlIdentifier))]
		[Rule("<OpenxmlColumnList> ::= <OpenxmlColumn> ',' <OpenxmlColumnList>", typeof(OpenxmlColumn), ConstructorParameterMapping = new[] {0, 2})]
		public Sequence(T item, Sequence<T> next) {
			this.next = next;
			this.item = item;
		}

		public T Item {
			get {
				return item;
			}
		}

		public Sequence<T> Next {
			get {
				return next;
			}
		}

		public IEnumerator<T> GetEnumerator() {
			for (Sequence<T> sequence = this; sequence != null; sequence = sequence.Next) {
				if (sequence.Item != null) {
					yield return sequence.Item;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}