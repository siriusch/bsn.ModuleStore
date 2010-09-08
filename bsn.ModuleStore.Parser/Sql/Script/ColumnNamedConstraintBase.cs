using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class ColumnNamedConstraintBase: ColumnConstraint {
		private readonly ConstraintName constraintName;

		protected ColumnNamedConstraintBase(ConstraintName constraintName) {
			this.constraintName = constraintName;
		}

		public ConstraintName ConstraintName {
			get {
				return constraintName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(constraintName, WhitespacePadding.SpaceAfter, "CONSTRAINT ", null);
		}
	}
}