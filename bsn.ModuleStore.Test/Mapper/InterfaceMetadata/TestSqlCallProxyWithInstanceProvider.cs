#if DEBUG

using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	[TestFixture]
	[Category("SQL")]
	public class TestSqlCallProxyWithInstanceProvider: TestSqlCallProxyBase {
		[TestFixtureSetUp]
		public void Setup() {
			SetupTests();
			DataProvider.Provider = new DistinctInstanceProvider<Guid>();
		}

		[TestFixtureTearDown]
		public void TearDown() {
			TeardownTests();
		}

		[Test(Description = "Tests the insert of multiple records using table value parameters")]
		public void TestTvpInsert()
		{
			RunTvpInsert();
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
				// Because of the instance provider, we expect to get all children registered on the right parent.
				// There should be only one parent object containing all of its children
				Assert.AreEqual(3, testChild.Parent.Children.Count);
			}
		}

		[Test(Description = "Tests parents with nested list of children marked with SqlDeserializeAttribute")]
		public void TestNestedObjectsWithEndDeserialization() {
			List<TestParentWithChildren> parents = DataProvider.spListParentWithChildren();
			Assert.AreEqual(2, parents.Count);
			foreach (TestParentWithChildren testParent in parents) {
				Assert.NotNull(testParent.Children);
				Assert.AreEqual(3, testParent.Children.Count);
				foreach (TestChildWithoutParent testChild in testParent.Children) {
					Assert.AreEqual(testParent.Id, testChild.ParentId);
				}
			}
		}

		[Test(Description = "Tests a sp call without a parameter and without input")]
		public void TestSimpleSpCall() {
			RunSimpleSpCall();
		}
	}
}

#endif
