using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace bsn.ModuleStore.Sql.Script {
	[TestFixture]
	public class SqlQualifiedNameTest: AssertionHelper {
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
		public void CompareToIdentical() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Expect(x.CompareTo(y), EqualTo(0));
		}

		[Test]
		public void CompareToEquivalent() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("DBO"), new TableName("ABC"));
			Expect(x.CompareTo(y), EqualTo(0));
		}

		[Test]
		public void CompareToSimilar() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, FunctionName>(new SchemaName("dbo"), new FunctionName("abc"));
			Expect(x.CompareTo(y), GreaterThan(0));
		}

		[Test]
		public void CompareToLargerSchema() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("xyz"), new TableName("abc"));
			Expect(x.CompareTo(y), LessThan(0));
		}

		[Test]
		public void CompareToLargerName() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("xyz"));
			Expect(x.CompareTo(y), LessThan(0));
		}

		[Test]
		public void CompareToSmallerSchema() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("xyz"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Expect(x.CompareTo(y), GreaterThan(0));
		}

		[Test]
		public void CompareToSmallerName() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("xyz"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Expect(x.CompareTo(y), GreaterThan(0));
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
		public void EqualsToIdentical() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			Expect(x.Equals(y), True);
		}

		[Test]
		public void EqualsToEquivalent() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("DBO"), new TableName("ABC"));
			Expect(x.Equals(y), True);
		}

		[Test]
		public void EqualsToSimilar() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, FunctionName>(new SchemaName("dbo"), new FunctionName("abc"));
			Expect(x.Equals(y), False);
		}

		[Test]
		public void EqualsToDifferentSchema() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("xyz"), new TableName("abc"));
			Expect(x.Equals(y), False);
		}

		[Test]
		public void EqualsToDifferentName() {
			IQualifiedName<SchemaName> x = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("abc"));
			IQualifiedName<SchemaName> y = new Qualified<SchemaName, TableName>(new SchemaName("dbo"), new TableName("xyz"));
			Expect(x.Equals(y), False);
		}
	}
}
