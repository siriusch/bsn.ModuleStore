using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ColumnCheckConstraint: ColumnNamedConstraintBase {
		[Rule("<NamedColumnConstraint> ::= CHECK <OptionalNotForReplication> '(' <Expression> ')'", ConstructorParameterMapping = new[] {1, 3})]
		public ColumnCheckConstraint(Optional<ForReplication> notForReplication, Expression expression): this(null, notForReplication, expression) {}

		[Rule("<NamedColumnConstraint> ::= CONSTRAINT <ConstraintName> CHECK <OptionalNotForReplication> '(' <Expression> ')'", ConstructorParameterMapping = new[] {1, 3, 5})]
		public ColumnCheckConstraint(ConstraintName constraintName, Optional<ForReplication> notForReplication, Expression expression): base(constraintName) {}
	}
}