using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public enum ObjectCategory {
		None,
		Table,
		View,
		Trigger,
		Index,
		Function,
		Procedure,
		XmlSchema
	}
}