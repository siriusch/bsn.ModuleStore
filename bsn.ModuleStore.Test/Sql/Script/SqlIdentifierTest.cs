﻿// bsn ModuleStore database versioning
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
	public class SqlIdentifierTest {
		[Fact]
		public void TryDequoteBracket() {
			string result;
			Assert.True(QuotedIdentifier.TryDequote("[Cool \" Stuff]", out result));
			Assert.Equal("Cool \" Stuff", result);
		}

		[Fact]
		public void TryDequoteQuote() {
			string result;
			Assert.True(QuotedIdentifier.TryDequote("\"Cool [ Stuff\"", out result));
			Assert.Equal("Cool [ Stuff", result);
		}

		[Fact]
		public void TryDequoteQuoteWithInnerQuote() {
			string result;
			Assert.True(QuotedIdentifier.TryDequote("\"Cool \"\" Stuff\"", out result));
			Assert.Equal("Cool \" Stuff", result);
		}
	}
}
