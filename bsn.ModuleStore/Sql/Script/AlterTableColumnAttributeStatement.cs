using System;
using System.IO;

using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class AlterTableColumnAttributeStatement: AlterTableColumnStatement {
		private readonly DdlOperation ddlOperation;

		protected AlterTableColumnAttributeStatement(TableName tableName, ColumnName columnName, DdlOperationToken ddlOperationToken): base(tableName, columnName) {
			if (ddlOperationToken == null) {
				throw new ArgumentNullException("ddlOperationToken");
			}
			ddlOperation = ddlOperationToken.Operation;
		}

		public override void WriteTo(TextWriter writer) {
			base.WriteTo(writer);
			writer.Write(ddlOperation);
			writer.Write(' ');
		}
	}
}