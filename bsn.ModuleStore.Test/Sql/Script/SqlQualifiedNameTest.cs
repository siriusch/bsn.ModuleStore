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
	public class SqlQualifiedNameTest: AssertionHelper {
		[Test]
		public void CompareToEquivalent() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("DBO"), new TableName("ABC"));
			Expect(x.CompareTo(y), EqualTo(0));
		}

		[Test]
		public void CompareToIdentical() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Expect(x.CompareTo(y), EqualTo(0));
		}

		[Test]
		public void CompareToLargerName() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("xyz"));
			Expect(x.CompareTo(y), LessThan(0));
		}

		[Test]
		public void CompareToLargerSchema() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("xyz"), new TableName("abc"));
			Expect(x.CompareTo(y), LessThan(0));
		}

		[Test]
		public void CompareToNull() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Expect(x.CompareTo(null), GreaterThan(0));
		}

		[Test]
		public void CompareToSelf() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Expect(x.CompareTo(x), EqualTo(0));
		}

		[Test]
		public void CompareToSimilar() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, FunctionName>(new SchemaName("dbo"), new FunctionName("abc"));
			Expect(x.CompareTo(y), GreaterThan(0));
		}

		[Test]
		public void CompareToSmallerName() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("xyz"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Expect(x.CompareTo(y), GreaterThan(0));
		}

		[Test]
		public void CompareToSmallerSchema() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("xyz"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Expect(x.CompareTo(y), GreaterThan(0));
		}

		[Test]
		public void EqualsToDifferentName() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("xyz"));
			Expect(x.Equals(y), False);
		}

		[Test]
		public void EqualsToDifferentSchema() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("xyz"), new TableName("abc"));
			Expect(x.Equals(y), False);
		}

		[Test]
		public void EqualsToEquivalent() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("DBO"), new TableName("ABC"));
			Expect(x.Equals(y), True);
		}

		[Test]
		public void EqualsToIdentical() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Expect(x.Equals(y), True);
		}

		[Test]
		public void EqualsToNull() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Expect(x.Equals(null), False);
		}

		[Test]
		public void EqualsToSelf() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Expect(x.Equals(x), True);
		}

		[Test]
		public void EqualsToSimilar() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, FunctionName>(new SchemaName("dbo"), new FunctionName("abc"));
			Expect(x.Equals(y), False);
		}
	}
}
