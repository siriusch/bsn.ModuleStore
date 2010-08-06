using System;
using System.Collections.Generic;

using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class TableUniqueConstraintBase: TableConstraint {
		private readonly Clustered clustered;
		private readonly ConstraintIndex constraintIndex;
		private readonly List<IndexColumn> indexColumns;

		protected TableUniqueConstraintBase(ConstraintName constraintName, ConstraintClusterToken clustered, Sequence<IndexColumn> indexColumns, ConstraintIndex constraintIndex): base(constraintName) {
			this.constraintIndex = constraintIndex;
			this.clustered = clustered.Clustered;
			this.indexColumns = indexColumns.ToList();
		}
	}
}