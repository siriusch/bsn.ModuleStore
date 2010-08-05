using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ColumnForeignKeyConstraint: ColumnNamedConstraintBase {
		[Rule("<NamedColumnConstraint> ::= REFERENCES <TableName> <OptionalForeignRefColumn> <ForeignKeyActionList>", ConstructorParameterMapping = new[] {1, 2, 3})]
		[Rule("<NamedColumnConstraint> ::= FOREIGN KEY REFERENCES <TableName> <OptionalForeignRefColumn> <ForeignKeyActionList>", ConstructorParameterMapping = new[] {3, 4, 5})]
		public ColumnForeignKeyConstraint(TableName tableName, Optional<ColumnName> refColumnName, Sequence<ForeignKeyAction> keyActions): this(null, tableName, refColumnName, keyActions) {}

		[Rule("<NamedColumnConstraint> ::= CONSTRAINT <ConstraintName> REFERENCES <TableName> <OptionalForeignRefColumn> <ForeignKeyActionList>", ConstructorParameterMapping = new[] {1, 3, 4, 5})]
		[Rule("<NamedColumnConstraint> ::= CONSTRAINT <ConstraintName> FOREIGN KEY REFERENCES <TableName> <OptionalForeignRefColumn> <ForeignKeyActionList>", ConstructorParameterMapping = new[] {1, 5, 6, 7})]
		public ColumnForeignKeyConstraint(ConstraintName constraintName, TableName tableName, Optional<ColumnName> refColumnName, Sequence<ForeignKeyAction> keyActions): base(constraintName) {}
	}
}