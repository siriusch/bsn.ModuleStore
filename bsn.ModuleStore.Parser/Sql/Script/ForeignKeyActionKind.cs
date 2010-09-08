using System;

namespace bsn.ModuleStore.Sql.Script {
	public enum ForeignKeyActionKind {
		None,
		Cascade,
		SetNull,
		SetDefault
	}
}