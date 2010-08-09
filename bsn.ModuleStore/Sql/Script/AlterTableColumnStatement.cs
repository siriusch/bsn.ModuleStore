using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class AlterTableColumnStatement: AlterTableStatement {
		private readonly ColumnName columnName;

		protected AlterTableColumnStatement(TableName tableName, ColumnName columnName): base(tableName) {
			if (columnName == null) {
				throw new ArgumentNullException("columnName");
			}
			TableColumnDefinition.AssertIsNotWildcard(columnName);
			this.columnName = columnName;
		}

		public override void WriteTo(TextWriter writer) {
			throw new NotImplementedException();
		}
	}
}