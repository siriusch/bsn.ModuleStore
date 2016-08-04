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

using Xunit;

namespace bsn.ModuleStore.Sql.Script {
	public class StringLiteralTest {
		[Fact]
		public void ParseIsUnicodeFalse() {
			Assert.False(StringLiteral.ParseIsUnicode("'Plain ol'' ASCII'"));
		}

		[Fact]
		public void ParseIsUnicodeNull() {
			Assert.Throws<NullReferenceException>(() => StringLiteral.ParseIsUnicode(null))
					;
		}

		[Fact]
		public void ParseIsUnicodeTrueLower() {
			Assert.True(StringLiteral.ParseIsUnicode("n'This isn''t ANSI! äöü'"));
		}

		[Fact]
		public void ParseIsUnicodeTrueUpper() {
			Assert.True(StringLiteral.ParseIsUnicode("N'This isn''t ANSI! äöü'"));
		}

		[Fact]
		public void ParseValueAscii() {
			Assert.Equal("Plain ol' ASCII", StringLiteral.ParseValue("'Plain ol'' ASCII'"));
		}

		[Fact]
		public void ParseValueEmptyAscii() {
			Assert.Equal("", StringLiteral.ParseValue("''"));
		}

		[Fact]
		public void ParseValueEmptyUnicode() {
			Assert.Equal("", StringLiteral.ParseValue("N''"));
		}

		[Fact]
		public void ParseValueEscapeEnd() {
			Assert.Equal("End'", StringLiteral.ParseValue("'End'''"));
		}

		[Fact]
		public void ParseValueEscapeOnlyAscii() {
			Assert.Equal("'", StringLiteral.ParseValue("''''"));
		}

		[Fact]
		public void ParseValueEscapeOnlyUnicode() {
			Assert.Equal("'", StringLiteral.ParseValue("N''''"));
		}

		[Fact]
		public void ParseValueEscapeStartAscii() {
			Assert.Equal("'Start", StringLiteral.ParseValue("'''Start'"));
		}

		[Fact]
		public void ParseValueEscapeStartUnicode() {
			Assert.Equal("'Start", StringLiteral.ParseValue("N'''Start'"));
		}

		[Fact]
		public void ParseValueNull() {
			Assert.Throws<NullReferenceException>(() => StringLiteral.ParseIsUnicode(null));
			;
		}

		[Fact]
		public void ParseValueSimple() {
			Assert.Equal("A string", StringLiteral.ParseValue("'A string'"));
		}

		[Fact]
		public void ParseValueUnicode() {
			Assert.Equal("This isn't ANSI! äöü", StringLiteral.ParseValue("N'This isn''t ANSI! äöü'"));
		}
	}
}
