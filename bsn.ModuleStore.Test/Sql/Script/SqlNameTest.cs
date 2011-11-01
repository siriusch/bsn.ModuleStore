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
	public class SqlNameTest: AssertionHelper {
		[Test]
		public void CompareToEquivalent() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("ABC");
			Expect(x.CompareTo(y), EqualTo(0));
		}

		[Test]
		public void CompareToIdentical() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("abc");
			Expect(x.CompareTo(y), EqualTo(0));
		}

		[Test]
		public void CompareToLarger() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("xyz");
			Expect(x.CompareTo(y), LessThan(0));
		}

		[Test]
		public void CompareToNull() {
			SqlName x = new TableName("abc");
			Expect(x.CompareTo(null), GreaterThan(0));
		}

		[Test]
		public void CompareToSelf() {
			SqlName x = new TableName("abc");
			Expect(x.CompareTo(x), EqualTo(0));
		}

		[Test]
		public void CompareToSimilar() {
			SqlName x = new TableName("abc");
			SqlName y = new CollationName("abc");
			Expect(x.CompareTo(y), GreaterThan(0));
		}

		[Test]
		public void CompareToSmaller() {
			SqlName x = new TableName("xyz");
			SqlName y = new TableName("abc");
			Expect(x.CompareTo(y), GreaterThan(0));
		}

		[Test]
		public void EqualsToDifferent() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("xyz");
			Expect(x.Equals(y), False);
		}

		[Test]
		public void EqualsToEquivalent() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("ABC");
			Expect(x.Equals(y), True);
		}

		[Test]
		public void EqualsToIdentical() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("abc");
			Expect(x.Equals(y), True);
		}

		[Test]
		public void EqualsToNull() {
			SqlName x = new TableName("abc");
			Expect(x.Equals(null), False);
		}

		[Test]
		public void EqualsToSelf() {
			SqlName x = new TableName("abc");
			Expect(x.Equals(x), True);
		}

		[Test]
		public void EqualsToSimilar() {
			SqlName x = new TableName("abc");
			SqlName y = new CollationName("abc");
			Expect(x.Equals(y), False);
		}
	}
}
