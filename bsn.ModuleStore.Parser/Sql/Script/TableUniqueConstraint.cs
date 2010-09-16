using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TableUniqueConstraint: TableUniqueConstraintBase {
		[Rule("<TableConstraint> ::= ~UNIQUE <ConstraintCluster> ~'(' <IndexColumnList> ~')' <ConstraintIndex>")]
		public TableUniqueConstraint(ConstraintClusterToken clustered, Sequence<IndexColumn> indexColumns, ConstraintIndex constraintIndex): this(null, clustered, indexColumns, constraintIndex) {}

		[Rule("<TableConstraint> ::= ~CONSTRAINT <ConstraintName> ~UNIQUE <ConstraintCluster> ~'(' <IndexColumnList> ~')' <ConstraintIndex>")]
		public TableUniqueConstraint(ConstraintName constraintName, ConstraintClusterToken clustered, Sequence<IndexColumn> indexColumns, ConstraintIndex constraintIndex): base(constraintName, clustered, indexColumns, constraintIndex) {}

		protected override string UniqueKindName {
			get {
				return "UNIQUE";
			}
		}
	}
}