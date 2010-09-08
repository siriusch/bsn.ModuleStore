using System;

namespace bsn.ModuleStore.Sql.Script {
	public enum FulltextChangeTracking {
		Unspecified,
		Off,
		OffNoPopulation,
		Manual,
		Auto
	}
}