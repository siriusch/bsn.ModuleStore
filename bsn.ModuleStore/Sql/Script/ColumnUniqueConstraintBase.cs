using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class ColumnUniqueConstraintBase: ColumnNamedConstraintBase {
		protected ColumnUniqueConstraintBase(ConstraintName constraintName, Clustered clustered, ConstraintIndex constraintIndex): base(constraintName) {}
	}
}