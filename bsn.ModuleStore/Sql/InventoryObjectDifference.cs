using System;

namespace bsn.ModuleStore.Sql {
	public enum InventoryObjectDifference {
		None,
		Different,
		SourceOnly,
		TargetOnly
	}
}