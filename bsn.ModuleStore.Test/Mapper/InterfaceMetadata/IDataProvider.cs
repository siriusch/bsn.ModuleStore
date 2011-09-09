#if DEBUG

using System;
using System.Collections.Generic;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	internal interface IDataProvider: IStoredProcedures {
		// ReSharper disable InconsistentNaming
		void spAddSimpleTypes(Guid uidKey, string sKey, int iData);

		void spClearSimpleTypes();

		string spGetKey(Guid uidKey);

		[SqlProc("spListSimpleTypes")]
		SimpleTestData spGetSimpleType(string sKey);

		List<TestChild> spListParentChild();

		[SqlProc("spListParentChild")]
		List<TestParentWithChildren> spListParentWithChildren();

		[SqlProc("spListSimpleTypes")]
		List<SimpleTestData> spListSimpleTypes(string sKey);

		void spSaveSimpleTypes(IEnumerable<SimpleTestData> tblInput);

		// ReSharper restore InconsistentNaming
	}
}

#endif
