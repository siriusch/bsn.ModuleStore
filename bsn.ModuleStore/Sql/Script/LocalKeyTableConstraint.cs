namespace bsn.ModuleStore.Sql.Script {
	public abstract class LocalKeyTableConstraint: TableConstraint {
		protected LocalKeyTableConstraint(ConstraintName constraintName, Clustered clustered, Sequence<IndexColumn> indexColumns, ConstraintIndex constraintIndex) : base(constraintName) {
		}
	}
}