using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class AlterTableColumnPersistedStatement: AlterTableColumnAttributeStatement {
		[Rule("<AlterTableStatement> ::= ALTER TABLE <TableNameQualified> ALTER COLUMN <ColumnName> ADD_PERSISTED", ConstructorParameterMapping = new[] {2, 5, 6})]
		[Rule("<AlterTableStatement> ::= ALTER TABLE <TableNameQualified> ALTER COLUMN <ColumnName> DROP_PERSISTED", ConstructorParameterMapping = new[] {2, 5, 6})]
		public AlterTableColumnPersistedStatement(Qualified<SchemaName, TableName> tableName, ColumnName columnName, DdlOperationToken ddlOperationToken): base(tableName, columnName, ddlOperationToken) {}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("PERSISTED");
		}
	}
}