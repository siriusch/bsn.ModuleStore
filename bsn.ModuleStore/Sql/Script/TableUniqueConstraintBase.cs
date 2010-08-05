using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class TableUniqueConstraintBase: TableConstraint {
		protected TableUniqueConstraintBase(ConstraintName constraintName, Clustered clustered, Sequence<IndexColumn> indexColumns, ConstraintIndex constraintIndex): base(constraintName) {}
	}
}