using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnUniqueConstraint: ColumnUniqueConstraintBase {
		[Rule("<NamedColumnConstraint> ::= ~UNIQUE <ConstraintCluster> <ConstraintIndex>")]
		public ColumnUniqueConstraint(ConstraintClusterToken clustered, ConstraintIndex constraintIndex): this(null, clustered, constraintIndex) {}

		[Rule("<NamedColumnConstraint> ::= ~CONSTRAINT <ConstraintName> ~UNIQUE <ConstraintCluster> <ConstraintIndex>")]
		public ColumnUniqueConstraint(ConstraintName constraintName, ConstraintClusterToken clustered, ConstraintIndex constraintIndex): base(constraintName, clustered, constraintIndex) {}

		protected override string UniqueKindName {
			get {
				return "UNIQUE";
			}
		}
	}
}