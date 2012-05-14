#if DEBUG

using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	[TestFixture]
	[Category("SQL")]
	public class TestSqlCallProxy: TestSqlCallProxyBase {
		[TestFixtureSetUp]
		public void Setup() {
			SetupTests();
		}

		[TestFixtureTearDown]
		public void TearDown() {
			TeardownTests();
		}

		[Test(Description = "Tests a sp call and receive a list of the table value")]
		public void TestGetList() {
			RunGetList();
		}

		[Test(Description = "Test the deserialized return value of a sp")]
		public void TestGetObject() {
			RunGetObject();
		}

		[Test(Description = "Test the scalarreturn of a sp")]
		public void TestGetValue() {
			RunGetValue();
		}

		[Test(Description = "Tests a sp call and passes some parameters to the sp")]
		public void TestInsert() {
			RunInsert();
		}

		[Test(Description = "Tests nested objects marked with SqlDeserializeAttribute")]
		public void TestNestedObjects() {
			List<TestChild> children = DataProvider.spListParentChild();
			Assert.AreEqual(6, children.Count);
			foreach (TestChild testChild in children) {
				Assert.NotNull(testChild.Parent);
				// even if the cildren register themselvse with the parent,
				// we will get here allway one entry on the parent because we do not use an instanceprovider.
				// There will be a new parent object all the time if we do not use an instanceprovider!
				Assert.AreEqual(1, testChild.Parent.Children.Count);
			}
		}

		[Test(Description = "Tests a sp call without a parameter and without input")]
		public void TestSimpleSpCall() {
			RunSimpleSpCall();
		}

		[Test(Description = "Tests the insert of multiple records using table value parameters")]
		public void TestTvpInsert() {
			RunTvpInsert();
		}

		[Test(Description = "Tests the insert of multiple records using table value parameters and only one primitive column")]
		public void TestPrimitiveTvpInsert()
		{
			RunPrimitiveTvpInsert();
		}
	}
}

#endif
