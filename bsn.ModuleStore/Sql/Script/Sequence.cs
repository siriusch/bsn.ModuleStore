using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class Sequence<T>: SqlToken, IEnumerable<T> where T: SqlToken {
		private readonly T item;
		private readonly Sequence<T> next;

		[Rule("<ColumnNameList> ::= <ColumnName>", typeof(Name))]
		public Sequence(T item): this(item, null) {}

		[Rule("<ColumnNameList> ::= <ColumnNameList> ',' <ColumnName>", typeof(Name))]
		public Sequence(Sequence<T> next, InsignificantToken dummy, T item): this(item, next) {}

		private Sequence(T item, Sequence<T> next) {
			if (item == null) {
				throw new ArgumentNullException("item");
			}
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
				yield return sequence.Item;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
