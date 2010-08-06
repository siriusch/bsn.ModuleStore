using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class AlterTableColumnNotForReplicationStatement: AlterTableColumnAttributeStatement {
		[Rule("<AlterTableStatement> ::= ALTER TABLE <TableName> ALTER COLUMN <ColumnName> ADD NOT FOR_REPLICATION", ConstructorParameterMapping = new[] {2, 5, 6})]
		[Rule("<AlterTableStatement> ::= ALTER TABLE <TableName> ALTER COLUMN <ColumnName> DROP NOT FOR_REPLICATION", ConstructorParameterMapping = new[] {2, 5, 6})]
		public AlterTableColumnNotForReplicationStatement(TableName tableName, ColumnName columnName, DdlOperationToken ddlOperationToken): base(tableName, columnName, ddlOperationToken) {}
	}
}