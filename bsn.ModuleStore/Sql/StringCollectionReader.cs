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
//  
using System;
using System.Collections.Specialized;
using System.IO;

namespace bsn.ModuleStore.Sql {
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
