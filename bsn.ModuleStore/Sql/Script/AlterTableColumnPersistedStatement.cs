using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class AlterTableColumnPersistedStatement: AlterTableColumnAttributeStatement {
		[Rule("<AlterTableStatement> ::= ALTER TABLE <TableName> ALTER COLUMN <ColumnName> ADD_PERSISTED", ConstructorParameterMapping = new[] {2, 5, 6})]
		[Rule("<AlterTableStatement> ::= ALTER TABLE <TableName> ALTER COLUMN <ColumnName> DROP_PERSISTED", ConstructorParameterMapping=new[] { 2, 5, 6 })]
		public AlterTableColumnPersistedStatement(TableName tableName, ColumnName columnName, DdlOperation ddlOperation) : base(tableName, columnName, ddlOperation) {
		}
	}
}