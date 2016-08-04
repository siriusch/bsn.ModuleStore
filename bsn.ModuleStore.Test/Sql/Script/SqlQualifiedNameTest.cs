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
	public class SqlQualifiedNameTest {
		[Fact]
		public void CompareToEquivalent() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("DBO"), new TableName("ABC"));
			Assert.Equal(0, x.CompareTo(y));
		}

		[Fact]
		public void CompareToIdentical() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Assert.Equal(0, x.CompareTo(y));
		}

		[Fact]
		public void CompareToLargerName() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("xyz"));
			Assert.True(x.CompareTo(y) < 0);
		}

		[Fact]
		public void CompareToLargerSchema() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("xyz"), new TableName("abc"));
			Assert.True(x.CompareTo(y) < 0);
		}

		[Fact]
		public void CompareToNull() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Assert.True(x.CompareTo(null) > 0);
		}

		[Fact]
		public void CompareToSelf() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Assert.Equal(0, x.CompareTo(x));
		}

		[Fact]
		public void CompareToSimilar() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, FunctionName>(new SchemaName("dbo"), new FunctionName("abc"));
			Assert.True(x.CompareTo(y) > 0);
		}

		[Fact]
		public void CompareToSmallerName() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("xyz"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Assert.True(x.CompareTo(y) > 0);
		}

		[Fact]
		public void CompareToSmallerSchema() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("xyz"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Assert.True(x.CompareTo(y) > 0);
		}

		[Fact]
		public void EqualsToDifferentName() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("xyz"));
			Assert.False(x.Equals(y));
		}

		[Fact]
		public void EqualsToDifferentSchema() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("xyz"), new TableName("abc"));
			Assert.False(x.Equals(y));
		}

		[Fact]
		public void EqualsToEquivalent() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("DBO"), new TableName("ABC"));
			Assert.True(x.Equals(y));
		}

		[Fact]
		public void EqualsToIdentical() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Assert.True(x.Equals(y));
		}

		[Fact]
		public void EqualsToNull() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Assert.False(x.Equals(null));
		}

		[Fact]
		public void EqualsToSelf() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Assert.True(x.Equals(x));
		}

		[Fact]
		public void EqualsToSimilar() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, FunctionName>(new SchemaName("dbo"), new FunctionName("abc"));
			Assert.False(x.Equals(y));
		}
	}
}
