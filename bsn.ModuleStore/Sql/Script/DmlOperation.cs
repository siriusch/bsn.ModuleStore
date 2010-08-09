using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bsn.ModuleStore.Sql.Script {
	public enum DmlOperation {
		None,
		Select,
		Insert,
		Update,
		Delete
	}
}
