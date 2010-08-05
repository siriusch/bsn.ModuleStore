using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TableForeignKeyConstraint: TableConstraint {
		[Rule("<TableConstraint> ::= FOREIGN KEY '(' <ColumnNameList> ')' REFERENCES <TableName> <ColumnNameGroup> <ForeignKeyActionList>", ConstructorParameterMapping = new[] {3, 6, 7, 8})]
		public TableForeignKeyConstraint(Sequence<ColumnName> columnNames, TableName refTableName, Optional<Sequence<ColumnName>> refColumnNames, Sequence<ForeignKeyAction> keyActions): this(null, columnNames, refTableName, refColumnNames, keyActions) {}

		[Rule("<TableConstraint> ::= CONSTRAINT <ConstraintName> FOREIGN KEY '(' <ColumnNameList> ')' REFERENCES <TableName> <ColumnNameGroup> <ForeignKeyActionList>", ConstructorParameterMapping = new[] {1, 5, 8, 9, 10})]
		public TableForeignKeyConstraint(ConstraintName constraintName, Sequence<ColumnName> columnNames, TableName refTableName, Optional<Sequence<ColumnName>> refColumnNames, Sequence<ForeignKeyAction> keyActions): base(constraintName) {}
	}
}