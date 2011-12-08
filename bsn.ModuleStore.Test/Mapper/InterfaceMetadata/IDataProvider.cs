#if DEBUG

using System;
using System.Collections.Generic;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	internal interface IDataProvider: IStoredProcedures {
		// ReSharper disable InconsistentNaming
		void spAddSimpleTypes(Guid uidKey, string sKey, int? iData);

		void spClearSimpleTypes();

		string spGetKey(Guid uidKey);

		[SqlProc("spListSimpleTypes")]
		SimpleTestData spGetSimpleType(string sKey);

		List<TestChild> spListParentChild();

		ResultSet<TestChildMultiResultsets, ResultSet<TestParentMultiResultsets>> spListParentChildMultiResultsChildParent();

		ResultSet<TestChildWithoutParent, ResultSet<TestParent>> spListParentChildMultiResultsWithoutRelation();

		[SqlProc("spListParentChild")]
		List<TestParentWithChildren> spListParentWithChildren();

		[SqlProc("spListParentChildWithNull")]
		List<TestParentWithChildren> spListParentWithChildrenWithNull();

		[SqlProc("spListSimpleTypes")]
		List<SimpleTestData> spListSimpleTypes(string sKey);

		void spSaveSimpleTypes(IEnumerable<SimpleTestData> tblInput);

		// ReSharper restore InconsistentNaming
	}
}

#endif
