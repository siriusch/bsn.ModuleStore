using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public enum DdlOperation {
		None,
		Create,
		Drop,
		Alter,
		Add
	}
}