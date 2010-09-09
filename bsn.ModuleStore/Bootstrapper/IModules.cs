using System;
using System.Linq;

using bsn.ModuleStore.Mapper;

namespace bsn.ModuleStore.Bootstrapper {
	internal interface IModules {
		[SqlProcedure("spModuleAdd.sql")]
		Module Add(Guid id, Guid assemblyId, string schemaPrefix, string assemblyName);

		[SqlProcedure("spModuleDelete.sql", UseReturnValue = SqlReturnValue.Scalar)]
		bool Delete(Guid id);

		[SqlProcedure("spModuleList.sql")]
		Module[] List(Guid assemblyGuid);

		[SqlProcedure("spModuleUpdate.sql", UseReturnValue = SqlReturnValue.Scalar)]
		bool Update(Guid id, string assemblyName, byte[] setupHash, int updateVersion);
	}
}