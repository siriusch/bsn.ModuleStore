using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public enum TriggerType {
		None,
		For,
		InsteadOf,
		After
	}
}