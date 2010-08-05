namespace bsn.ModuleStore.Sql.Script {
	public abstract class ColumnNamedConstraintBase: ColumnConstraint {
		private readonly ConstraintName constraintName;

		protected ColumnNamedConstraintBase(ConstraintName constraintName) {
			this.constraintName = constraintName;
		}
	}
}