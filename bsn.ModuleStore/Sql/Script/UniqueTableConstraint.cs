using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class UniqueTableConstraint: LocalKeyTableConstraint {
		[Rule("<TableConstraint> ::= UNIQUE <ConstraintCluster> '(' <IndexColumnList> ')' <ConstraintIndex>", ConstructorParameterMapping = new[] {1, 3, 5})]
		public UniqueTableConstraint(Clustered clustered, Sequence<IndexColumn> indexColumns, ConstraintIndex constraintIndex) : this(null, clustered, indexColumns, constraintIndex) {
		}

		[Rule("<TableConstraint> ::= CONSTRAINT <ConstraintName> UNIQUE <ConstraintCluster> '(' <IndexColumnList> ')' <ConstraintIndex>", ConstructorParameterMapping = new[] {1, 3, 5, 7})]
		public UniqueTableConstraint(ConstraintName constraintName, Clustered clustered, Sequence<IndexColumn> indexColumns, ConstraintIndex constraintIndex) : base(constraintName, clustered, indexColumns, constraintIndex) {
		}
	}
}