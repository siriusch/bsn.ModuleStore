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
	public class SqlNameTest {
		[Fact]
		public void CompareToEquivalent() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("ABC");
			Assert.Equal(0, x.CompareTo(y));
		}

		[Fact]
		public void CompareToIdentical() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("abc");
			Assert.Equal(0, x.CompareTo(y));
		}

		[Fact]
		public void CompareToLarger() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("xyz");
			Assert.True(x.CompareTo(y) < 0);
		}

		[Fact]
		public void CompareToNull() {
			SqlName x = new TableName("abc");
			Assert.True(x.CompareTo(null) > 0);
		}

		[Fact]
		public void CompareToSelf() {
			SqlName x = new TableName("abc");
			Assert.Equal(0, x.CompareTo(x));
		}

		[Fact]
		public void CompareToSimilar() {
			SqlName x = new TableName("abc");
			SqlName y = new CollationName("abc");
			Assert.True(x.CompareTo(y) > 0);
		}

		[Fact]
		public void CompareToSmaller() {
			SqlName x = new TableName("xyz");
			SqlName y = new TableName("abc");
			Assert.True(x.CompareTo(y) > 0);
		}

		[Fact]
		public void EqualsToDifferent() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("xyz");
			Assert.False(x.Equals(y));
		}

		[Fact]
		public void EqualsToEquivalent() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("ABC");
			Assert.True(x.Equals(y));
		}

		[Fact]
		public void EqualsToIdentical() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("abc");
			Assert.True(x.Equals(y));
		}

		[Fact]
		public void EqualsToNull() {
			SqlName x = new TableName("abc");
			Assert.False(x.Equals(null));
		}

		[Fact]
		public void EqualsToSelf() {
			SqlName x = new TableName("abc");
			Assert.True(x.Equals(x));
		}

		[Fact]
		public void EqualsToSimilar() {
			SqlName x = new TableName("abc");
			SqlName y = new CollationName("abc");
			Assert.False(x.Equals(y));
		}
	}
}
