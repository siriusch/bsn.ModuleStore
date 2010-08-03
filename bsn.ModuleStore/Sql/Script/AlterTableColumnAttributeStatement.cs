using System;

namespace bsn.ModuleStore.Sql.Script {
	public class AlterTableColumnAttributeStatement: AlterTableColumnStatement {
		private readonly DdlOperation ddlOperation;

		protected AlterTableColumnAttributeStatement(TableName tableName, ColumnName columnName, DdlOperation ddlOperation): base(tableName, columnName) {
			if (ddlOperation == null) {
				throw new ArgumentNullException("ddlOperation");
			}
			this.ddlOperation = ddlOperation;
		}
	}
}