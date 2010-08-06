using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class TablePrimaryKeyConstraint: TableUniqueConstraintBase {
		[Rule("<TableConstraint> ::= PRIMARY KEY <ConstraintCluster> '(' <IndexColumnList> ')' <ConstraintIndex>", ConstructorParameterMapping = new[] {2, 4, 6})]
		public TablePrimaryKeyConstraint(ConstraintClusterToken clustered, Sequence<IndexColumn> indexColumns, ConstraintIndex constraintIndex): this(null, clustered, indexColumns, constraintIndex) {}

		[Rule("<TableConstraint> ::= CONSTRAINT <ConstraintName> PRIMARY KEY <ConstraintCluster> '(' <IndexColumnList> ')' <ConstraintIndex>", ConstructorParameterMapping = new[] {1, 4, 6, 8})]
		public TablePrimaryKeyConstraint(ConstraintName constraintName, ConstraintClusterToken clustered, Sequence<IndexColumn> indexColumns, ConstraintIndex constraintIndex): base(constraintName, clustered, indexColumns, constraintIndex) {}
	}
}