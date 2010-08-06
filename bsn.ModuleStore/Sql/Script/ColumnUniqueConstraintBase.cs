using System;

using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class ColumnUniqueConstraintBase: ColumnNamedConstraintBase {
		private readonly Clustered clustered;
		private readonly ConstraintIndex constraintIndex;
		private readonly ConstraintName constraintName;

		protected ColumnUniqueConstraintBase(ConstraintName constraintName, ConstraintClusterToken clustered, ConstraintIndex constraintIndex): base(constraintName) {
			this.constraintName = constraintName;
			this.constraintIndex = constraintIndex;
			this.clustered = clustered.Clustered;
		}

		public Clustered Clustered {
			get {
				return clustered;
			}
		}

		public ConstraintIndex ConstraintIndex {
			get {
				return constraintIndex;
			}
		}

		public ConstraintName ConstraintName {
			get {
				return constraintName;
			}
		}
	}
}