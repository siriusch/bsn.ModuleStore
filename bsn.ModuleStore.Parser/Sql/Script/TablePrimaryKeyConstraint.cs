using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TablePrimaryKeyConstraint: TableUniqueConstraintBase {
		[Rule("<TableConstraint> ::= ~PRIMARY ~KEY <ConstraintCluster> ~'(' <IndexColumnList> ~')' <ConstraintIndex>")]
		public TablePrimaryKeyConstraint(ConstraintClusterToken clustered, Sequence<IndexColumn> indexColumns, ConstraintIndex constraintIndex): this(null, clustered, indexColumns, constraintIndex) {}

		[Rule("<TableConstraint> ::= ~CONSTRAINT <ConstraintName> ~PRIMARY ~KEY <ConstraintCluster> ~'(' <IndexColumnList> ~')' <ConstraintIndex>")]
		public TablePrimaryKeyConstraint(ConstraintName constraintName, ConstraintClusterToken clustered, Sequence<IndexColumn> indexColumns, ConstraintIndex constraintIndex): base(constraintName, clustered, indexColumns, constraintIndex) {}

		protected override string UniqueKindName {
			get {
				return "PRIMARY KEY";
			}
		}
	}
}