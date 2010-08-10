using System;
using System.Collections.Generic;
using System.IO;

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

		public List<IndexColumn> IndexColumns {
			get {
				return indexColumns;
			}
		}

		protected abstract string UniqueKindName {
			get;
		}

		public override void WriteTo(TextWriter writer) {
			base.WriteTo(writer);
			writer.Write(UniqueKindName);
			writer.Write(' ');
			writer.WriteValue(clustered, null, " ");
			writer.Write('(');
			writer.WriteSequence(indexColumns, null, ", ", null);
			writer.Write(')');
			writer.WriteScript(constraintIndex, " ", null);
		}
	}
}