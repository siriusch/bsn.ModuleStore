using System;
using System.Diagnostics;
using System.IO;

using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class AlterTableColumnAttributeStatement: AlterTableColumnStatement {
		private readonly DdlOperation ddlOperation;

		protected AlterTableColumnAttributeStatement(TableName tableName, ColumnName columnName, DdlOperationToken ddlOperationToken): base(tableName, columnName) {
			Debug.Assert(ddlOperationToken != null);
			ddlOperation = ddlOperationToken.Operation;
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.WriteValue(ddlOperation, null, null);
			writer.Write(' ');
		}
	}
}