using System;
using System.Linq;

using bsn.ModuleStore.Mapper;

namespace bsn.ModuleStore.Bootstrapper {
	internal interface IModules: IStoredProcedures {
		[SqlProcedure("spModuleAdd.sql")]
		Module Add(Guid? id, Guid assemblyId, string schemaPrefix, string assemblyName);

		[SqlProcedure("spModuleDelete.sql", UseReturnValue = SqlReturnValue.ReturnValue)]
		bool Delete(Guid id);

		[SqlProcedure("spModuleList.sql")]
		Module[] List(Guid assemblyGuid);

		[SqlProcedure("spModuleUpdate.sql", UseReturnValue = SqlReturnValue.ReturnValue)]
		bool Update(Guid id, string assemblyName, byte[] setupHash, int updateVersion);
	}
}