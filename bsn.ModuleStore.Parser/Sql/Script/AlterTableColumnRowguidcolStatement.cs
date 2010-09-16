using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class AlterTableColumnRowguidcolStatement: AlterTableColumnAttributeStatement {
		[Rule("<AlterTableStatement> ::= ~ALTER ~TABLE <TableNameQualified> ~ALTER ~COLUMN <ColumnName> ADD ~ROWGUIDCOL")]
		[Rule("<AlterTableStatement> ::= ~ALTER ~TABLE <TableNameQualified> ~ALTER ~COLUMN <ColumnName> DROP ~ROWGUIDCOL")]
		public AlterTableColumnRowguidcolStatement(Qualified<SchemaName, TableName> tableName, ColumnName columnName, DdlOperationToken ddlOperationToken): base(tableName, columnName, ddlOperationToken) {}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("ROWGUIDCOL");
		}
	}
}