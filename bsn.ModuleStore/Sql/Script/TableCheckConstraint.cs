using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class TableCheckConstraint: TableConstraint {
		[Rule("<TableConstraint> ::= CHECK <OptionalNotForReplication> '(' <Expression> ')'", ConstructorParameterMapping = new[] {1, 3})]
		public TableCheckConstraint(Optional<ForReplicationToken> notForReplication, Expression expression): this(null, notForReplication, expression) {}

		[Rule("<TableConstraint> ::= CONSTRAINT <ConstraintName> CHECK <OptionalNotForReplication> '(' <Expression> ')'", ConstructorParameterMapping = new[] {1, 3, 5})]
		public TableCheckConstraint(ConstraintName constraintName, Optional<ForReplicationToken> notForReplication, Expression expression): base(constraintName) {}
	}
}