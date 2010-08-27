using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	internal class StringCollectionReader: TextReader {
		private readonly string newLine;
		private readonly StringEnumerator stringEnumerator;
		private string current;
		private int index;

		public StringCollectionReader(StringCollection stringCollection, string newLine) {
			stringEnumerator = stringCollection.GetEnumerator();
			this.newLine = newLine ?? Environment.NewLine;
			GetNextLine();
		}

		public override int Peek() {
			return GetCharAt(index);
		}

		public override int Read() {
			int result = GetCharAt(index++);
			if (result >= 0) {
				if ((current != null) && (index >= current.Length)) {
					index -= (current.Length+newLine.Length);
					GetNextLine();
				}
			}
			return result;
		}

		private int GetCharAt(int index) {
			if (index < 0) {
				return newLine[newLine.Length+index];
			}
			if (current == null) {
				return -1;
			}
			if (index >= current.Length) {
				return newLine[index-current.Length];
			}
			return current[index];
		}

		private void GetNextLine() {
			current = stringEnumerator.MoveNext() ? stringEnumerator.Current : null;
		}
	}
}