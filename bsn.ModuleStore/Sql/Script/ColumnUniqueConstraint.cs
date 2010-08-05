using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ColumnUniqueConstraint: ColumnUniqueConstraintBase {
		[Rule("<NamedColumnConstraint> ::= UNIQUE <ConstraintCluster> <ConstraintIndex>", ConstructorParameterMapping = new[] {1, 2})]
		public ColumnUniqueConstraint(Clustered clustered, ConstraintIndex constraintIndex): this(null, clustered, constraintIndex) {}

		[Rule("<NamedColumnConstraint> ::= CONSTRAINT <ConstraintName> UNIQUE <ConstraintCluster> <ConstraintIndex>", ConstructorParameterMapping = new[] {1, 3, 4})]
		public ColumnUniqueConstraint(ConstraintName constraintName, Clustered clustered, ConstraintIndex constraintIndex): base(constraintName, clustered, constraintIndex) {}
	}
}