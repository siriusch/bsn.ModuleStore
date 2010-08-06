using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class AlterTableColumnRowguidcolStatement: AlterTableColumnAttributeStatement {
		[Rule("<AlterTableStatement> ::= ALTER TABLE <TableName> ALTER COLUMN <ColumnName> ADD ROWGUIDCOL", ConstructorParameterMapping = new[] {2, 5, 6})]
		[Rule("<AlterTableStatement> ::= ALTER TABLE <TableName> ALTER COLUMN <ColumnName> DROP ROWGUIDCOL", ConstructorParameterMapping = new[] {2, 5, 6})]
		public AlterTableColumnRowguidcolStatement(TableName tableName, ColumnName columnName, DdlOperationToken ddlOperationToken): base(tableName, columnName, ddlOperationToken) {}
	}
}