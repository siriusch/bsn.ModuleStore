using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class TableUniqueConstraint: TableUniqueConstraintBase {
		[Rule("<TableConstraint> ::= UNIQUE <ConstraintCluster> '(' <IndexColumnList> ')' <ConstraintIndex>", ConstructorParameterMapping = new[] {1, 3, 5})]
		public TableUniqueConstraint(ConstraintClusterToken clustered, Sequence<IndexColumn> indexColumns, ConstraintIndex constraintIndex): this(null, clustered, indexColumns, constraintIndex) {}

		[Rule("<TableConstraint> ::= CONSTRAINT <ConstraintName> UNIQUE <ConstraintCluster> '(' <IndexColumnList> ')' <ConstraintIndex>", ConstructorParameterMapping = new[] {1, 3, 5, 7})]
		public TableUniqueConstraint(ConstraintName constraintName, ConstraintClusterToken clustered, Sequence<IndexColumn> indexColumns, ConstraintIndex constraintIndex): base(constraintName, clustered, indexColumns, constraintIndex) {}
	}
}