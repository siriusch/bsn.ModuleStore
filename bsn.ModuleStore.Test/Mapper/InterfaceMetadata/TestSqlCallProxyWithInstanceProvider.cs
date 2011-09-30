#if DEBUG

using System;
using System.Collections.Generic;
using System.Diagnostics;

using NUnit.Framework;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	[TestFixture]
	[Category("SQL")]
	public class TestSqlCallProxyWithInstanceProvider: TestSqlCallProxyBase {
		[TestFixtureSetUp]
		public void Setup() {
			SetupTests();
			DataProvider.Provider = new SingleInstanceProvider<Guid>();
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

		[Test(Description = "Tests parents with nested list of children marked with SqlDeserializeAttribute and no null children")]
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

		[Test(Description = "Tests parents with nested list of children marked with SqlDeserializeAttribute with null children")]
		public void TestNestedObjectsWithEndDeserializationWithNullChildren()
		{
			List<TestParentWithChildren> parents = DataProvider.spListParentWithChildrenWithNull();
			Guid emptyChildrenParent = new Guid("F475E4B4-09F6-4334-8C1F-EB53E26B0280");
			Assert.AreEqual(3, parents.Count);
			foreach (TestParentWithChildren testParent in parents) {
				Assert.NotNull(testParent.Children);
				if (testParent.Id == emptyChildrenParent) {
					Assert.AreEqual(0, testParent.Children.Count);
				} else {
					Assert.AreEqual(3, testParent.Children.Count);
				}
				foreach (TestChildWithoutParent testChild in testParent.Children) {
					Assert.AreEqual(testParent.Id, testChild.ParentId);
				}
			}
		}

		[Test(Description = "Tests a sp call without a parameter and without input")]
		public void TestSimpleSpCall() {
			RunSimpleSpCall();
		}

		[Test(Description = "Tests multiple resultsets with parent and child without relation")]
		public void RunMulitResultSetParentChildNoRelation()
		{
			ResultSet<TestChildWithoutParent, ResultSet<TestParent>> result = DataProvider.spListParentChildMultiResultsWithoutRelation();
			Assert.AreEqual(6, result.Count);
			Assert.AreEqual(3, result.Inner.Count);
		}

		[Test(Description = "Tests multiple resultsets with child to parent relationship")]
		public void RunMulitResultSetParentChildNonCircularParent()
		{
			try {
				ResultSet<TestChildMultiResultsets, ResultSet<TestParentMultiResultsets>> result = DataProvider.spListParentChildMultiResultsChildParent();
				Assert.AreEqual(6, result.Count);
				Assert.AreEqual(2, result.Inner.Count);
			} catch (Exception ex) {
				Debug.WriteLine(ex.ToString());
				throw new ApplicationException("rethrown", ex);
			}

		}
	}
}

#endif
