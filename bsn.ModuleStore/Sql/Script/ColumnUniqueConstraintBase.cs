using System;
using System.IO;

using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class ColumnUniqueConstraintBase: ColumnNamedConstraintBase {
		private readonly Clustered clustered;
		private readonly ConstraintIndex constraintIndex;

		protected ColumnUniqueConstraintBase(ConstraintName constraintName, ConstraintClusterToken clustered, ConstraintIndex constraintIndex): base(constraintName) {
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

		protected abstract string UniqueKindName {
			get;
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write(UniqueKindName);
			writer.WriteValue(clustered, " ", null);
			writer.WriteScript(constraintIndex, " ", null);
		}
	}
}