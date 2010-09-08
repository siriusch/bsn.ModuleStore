using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace bsn.ModuleStore.Sql.Script {
	[TestFixture]
	public class SqlNameTest: AssertionHelper {
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
		public void CompareToIdentical() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("abc");
			Expect(x.CompareTo(y), EqualTo(0));
		}

		[Test]
		public void CompareToEquivalent() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("ABC");
			Expect(x.CompareTo(y), EqualTo(0));
		}

		[Test]
		public void CompareToSimilar() {
			SqlName x = new TableName("abc");
			SqlName y = new CollationName("abc");
			Expect(x.CompareTo(y), GreaterThan(0));
		}

		[Test]
		public void CompareToLarger() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("xyz");
			Expect(x.CompareTo(y), LessThan(0));
		}

		[Test]
		public void CompareToSmaller() {
			SqlName x = new TableName("xyz");
			SqlName y = new TableName("abc");
			Expect(x.CompareTo(y), GreaterThan(0));
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
		public void EqualsToIdentical() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("abc");
			Expect(x.Equals(y), True);
		}

		[Test]
		public void EqualsToEquivalent() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("ABC");
			Expect(x.Equals(y), True);
		}

    [Test]
		public void EqualsToSimilar() {
			SqlName x = new TableName("abc");
			SqlName y = new CollationName("abc");
			Expect(x.Equals(y), False);
		}

		[Test]
		public void EqualsToDifferent() {
			SqlName x = new TableName("abc");
			SqlName y = new TableName("xyz");
			Expect(x.Equals(y), False);
		}
	}
}
