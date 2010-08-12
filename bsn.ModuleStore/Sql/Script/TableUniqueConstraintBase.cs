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

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write(UniqueKindName);
			writer.Write(' ');
			writer.WriteEnum(clustered, WhitespacePadding.SpaceAfter);
			writer.Write('(');
			writer.IncreaseIndent();
			writer.WriteSequence(indexColumns, WhitespacePadding.NewlineBefore, ", ");
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write(')');
			writer.WriteScript(constraintIndex, WhitespacePadding.None);
		}
	}
}