using System;

namespace bsn.ModuleStore.Sql.Script {
	public enum FulltextChangeTrackingKind {
		Unspecified,
		Off,
		OffNoPopulation,
		Manual,
		Auto
	}
}