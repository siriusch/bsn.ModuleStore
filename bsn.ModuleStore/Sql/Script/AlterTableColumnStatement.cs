using System;
using System.Diagnostics;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class AlterTableColumnStatement: AlterTableStatement {
		private readonly ColumnName columnName;

		protected AlterTableColumnStatement(TableName tableName, ColumnName columnName): base(tableName) {
			Debug.Assert(columnName != null);
			TableColumnDefinition.AssertIsNotWildcard(columnName);
			this.columnName = columnName;
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("ALTER COLUMN ");
			writer.WriteScript(columnName);
			writer.Write(' ');
		}
	}
}