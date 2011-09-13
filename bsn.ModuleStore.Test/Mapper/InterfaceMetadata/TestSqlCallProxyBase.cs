#if DEBUG

using System;
using System.Collections.Generic;
using System.Diagnostics;

using NUnit.Framework;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	public class TestSqlCallProxyBase {
		private IDataProvider dataProvider;
		private NUnitSetup nUnitSetup;
		private List<SimpleTestData> testData;

		internal IDataProvider DataProvider {
			get {
				return dataProvider;
			}
		}

		internal NUnitSetup NUnitSetup {
			get {
				return nUnitSetup;
			}
		}

		internal List<SimpleTestData> TestData {
			get {
				return testData;
			}
		}

		protected void RunTvpInsert() {
			Debug.WriteLine("TestList: clearing table...");
			dataProvider.spClearSimpleTypes();
			Debug.WriteLine("TestList: adding testdata...");
			dataProvider.spSaveSimpleTypes(testData);
		}

		protected void RunGetList() {
			Debug.WriteLine("TestList: clearing table...");
			dataProvider.spClearSimpleTypes();
			Debug.WriteLine("TestList: adding testdata...");
			foreach (SimpleTestData simpleTestData in testData) {
				dataProvider.spAddSimpleTypes(simpleTestData.Id, simpleTestData.Key, simpleTestData.Data);
			}
			Debug.WriteLine("TestList: getting testdata...");
			List<SimpleTestData> simpleTestDatas = dataProvider.spListSimpleTypes(null);
			Assert.AreEqual(testData.Count, simpleTestDatas.Count);
			foreach (SimpleTestData t in testData) {
				Assert.True(simpleTestDatas.Contains(t));
			}
			foreach (SimpleTestData t in simpleTestDatas) {
				Assert.True(testData.Contains(t));
			}
		}

		protected void RunGetObject() {
			Debug.WriteLine("TestInsert: clearing table...");
			dataProvider.spClearSimpleTypes();
			Debug.WriteLine("TestInsert: adding one record...");
			dataProvider.spAddSimpleTypes(testData[0].Id, testData[0].Key, testData[0].Data);
			Debug.WriteLine("TestInsert: Getting scalar value from sp...");
			SimpleTestData data = dataProvider.spGetSimpleType(testData[0].Key);
			Assert.AreEqual(testData[0], data);
		}

		protected void RunGetValue() {
			Debug.WriteLine("TestInsert: clearing table...");
			dataProvider.spClearSimpleTypes();
			Debug.WriteLine("TestInsert: adding one record...");
			dataProvider.spAddSimpleTypes(testData[0].Id, testData[0].Key, testData[0].Data);
			Debug.WriteLine("TestInsert: Getting scalar value from sp...");
			string key = dataProvider.spGetKey(testData[0].Id);
			Assert.AreEqual(testData[0].Key, key);
		}

		protected void RunInsert() {
			Debug.WriteLine("TestInsert: clearing table...");
			dataProvider.spClearSimpleTypes();
			Debug.WriteLine("TestInsert: adding one record...");
			dataProvider.spAddSimpleTypes(testData[0].Id, testData[0].Key, testData[0].Data);
		}

		protected void RunSimpleSpCall() {
			Debug.WriteLine("TestSimpleSpCall: clearing table...");
			dataProvider.spClearSimpleTypes();
		}

		protected void SetupTests() {
			nUnitSetup = new NUnitSetup("Data Source=tmp3dev;Initial Catalog=SqlCallProxyNunitTest;Integrated Security=True;");
			nUnitSetup.SetupTestEnvironment();
			IConnectionProvider connectionProvider = new ConnectionProvider(nUnitSetup.ConnectionString);
			dataProvider = SqlCallProxy.Create<IDataProvider>(new ReflectionMetadataProvider(), connectionProvider);
			testData = new List<SimpleTestData>();
			for (int i = 0; i < 100; i++) {
				testData.Add(new SimpleTestData(Guid.NewGuid(), string.Format("key_{0}", i), i));
			}
		}

		protected void TeardownTests() {
			//this.nUnitSetup.CleanUpTestDatabase();
		}
	}
}

#endif
