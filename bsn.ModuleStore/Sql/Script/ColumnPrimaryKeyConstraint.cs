using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnPrimaryKeyConstraint: ColumnUniqueConstraintBase {
		[Rule("<NamedColumnConstraint> ::= PRIMARY KEY <ConstraintCluster> <ConstraintIndex>", ConstructorParameterMapping = new[] {2, 3})]
		public ColumnPrimaryKeyConstraint(ConstraintClusterToken clustered, ConstraintIndex constraintIndex): this(null, clustered, constraintIndex) {}

		[Rule("<NamedColumnConstraint> ::= CONSTRAINT <ConstraintName> PRIMARY KEY <ConstraintCluster> <ConstraintIndex>", ConstructorParameterMapping = new[] {1, 4, 5})]
		public ColumnPrimaryKeyConstraint(ConstraintName constraintName, ConstraintClusterToken clustered, ConstraintIndex constraintIndex): base(constraintName, clustered, constraintIndex) {}

		protected override string UniqueKindName {
			get {
				return "PRIMARY KEY";
			}
		}
	}
}