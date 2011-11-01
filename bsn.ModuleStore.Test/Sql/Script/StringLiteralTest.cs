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

using NUnit.Framework;

namespace bsn.ModuleStore.Sql.Script {
	[TestFixture]
	public class StringLiteralTest: AssertionHelper {
		[Test]
		public void ParseIsUnicodeFalse() {
			Expect(!StringLiteral.ParseIsUnicode("'Plain ol'' ASCII'"));
		}

		[Test]
		[ExpectedException(typeof(NullReferenceException))]
		public void ParseIsUnicodeNull() {
			StringLiteral.ParseIsUnicode(null);
		}

		[Test]
		public void ParseIsUnicodeTrueLower() {
			Expect(StringLiteral.ParseIsUnicode("n'This isn''t ANSI! äöü'"));
		}

		[Test]
		public void ParseIsUnicodeTrueUpper() {
			Expect(StringLiteral.ParseIsUnicode("N'This isn''t ANSI! äöü'"));
		}

		[Test]
		public void ParseValueAscii() {
			Expect(StringLiteral.ParseValue("'Plain ol'' ASCII'"), EqualTo("Plain ol' ASCII"));
		}

		[Test]
		public void ParseValueEmptyAscii() {
			Expect(StringLiteral.ParseValue("''"), EqualTo(""));
		}

		[Test]
		public void ParseValueEmptyUnicode() {
			Expect(StringLiteral.ParseValue("N''"), EqualTo(""));
		}

		[Test]
		public void ParseValueEscapeEnd() {
			Expect(StringLiteral.ParseValue("'End'''"), EqualTo("End'"));
		}

		[Test]
		public void ParseValueEscapeOnlyAscii() {
			Expect(StringLiteral.ParseValue("''''"), EqualTo("'"));
		}

		[Test]
		public void ParseValueEscapeOnlyUnicode() {
			Expect(StringLiteral.ParseValue("N''''"), EqualTo("'"));
		}

		[Test]
		public void ParseValueEscapeStartAscii() {
			Expect(StringLiteral.ParseValue("'''Start'"), EqualTo("'Start"));
		}

		[Test]
		public void ParseValueEscapeStartUnicode() {
			Expect(StringLiteral.ParseValue("N'''Start'"), EqualTo("'Start"));
		}

		[Test]
		[ExpectedException(typeof(NullReferenceException))]
		public void ParseValueNull() {
			StringLiteral.ParseIsUnicode(null);
		}

		[Test]
		public void ParseValueSimple() {
			Expect(StringLiteral.ParseValue("'A string'"), EqualTo("A string"));
		}

		[Test]
		public void ParseValueUnicode() {
			Expect(StringLiteral.ParseValue("N'This isn''t ANSI! äöü'"), EqualTo("This isn't ANSI! äöü"));
		}
	}
}
