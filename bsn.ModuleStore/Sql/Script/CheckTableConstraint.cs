using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CheckTableConstraint: TableConstraint {
		[Rule("<TableConstraint> ::= CHECK <OptionalNotForReplication> '(' <Expression> ')'", ConstructorParameterMapping = new[] {1, 3})]
		public CheckTableConstraint(Optional<ForReplication> notForReplication, Expression expression): this(null, notForReplication, expression) {}

		[Rule("<TableConstraint> ::= CONSTRAINT <ConstraintName> CHECK <OptionalNotForReplication> '(' <Expression> ')'", ConstructorParameterMapping = new[] {1, 3, 5})]
		public CheckTableConstraint(ConstraintName constraintName, Optional<ForReplication> notForReplication, Expression expression): base(constraintName) {}
	}
}