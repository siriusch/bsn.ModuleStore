using System;
using System.Linq;

using NUnit.Framework;

namespace bsn.ModuleStore.Sql {
	[TestFixture]
	public class DatabaseInventoryTest: AssertionHelper {
		[Test]
		public void TestIgnore() {
			throw new IgnoreException("Tgnore this test");
		}
	}
}