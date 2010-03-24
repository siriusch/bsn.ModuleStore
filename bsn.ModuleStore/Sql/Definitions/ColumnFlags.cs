// (C) 2010 Arsène von Wyss / bsn
using System;

namespace bsn.ModuleStore.Sql.Definitions {
	[Flags]
	public enum ColumnFlags {
		None = 0,
		NotNull = 1,
		Indentity = 2,
		RowGuid = 4,
		PrimaryKey = 8
	}
}