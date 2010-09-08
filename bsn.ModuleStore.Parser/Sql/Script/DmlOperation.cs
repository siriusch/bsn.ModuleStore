using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public enum DmlOperation {
		None,
		Select,
		Insert,
		Update,
		Delete
	}
}