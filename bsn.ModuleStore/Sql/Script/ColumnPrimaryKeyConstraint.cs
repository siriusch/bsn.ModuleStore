using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ColumnPrimaryKeyConstraint: ColumnUniqueConstraintBase {
		[Rule("<NamedColumnConstraint> ::= PRIMARY KEY <ConstraintCluster> <ConstraintIndex>", ConstructorParameterMapping = new[] {2, 3})]
		public ColumnPrimaryKeyConstraint(Clustered clustered, ConstraintIndex constraintIndex): this(null, clustered, constraintIndex) {}

		[Rule("<NamedColumnConstraint> ::= CONSTRAINT <ConstraintName> PRIMARY KEY <ConstraintCluster> <ConstraintIndex>", ConstructorParameterMapping = new[] {1, 4, 5})]
		public ColumnPrimaryKeyConstraint(ConstraintName constraintName, Clustered clustered, ConstraintIndex constraintIndex): base(constraintName, clustered, constraintIndex) {}
	}
}