using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class ColumnDefinition: TableDefinitionItem {
		private readonly ColumnName columnName;

		protected ColumnDefinition(ColumnName columnName) {
			if (columnName == null) {
				throw new ArgumentNullException("columnName");
			}
			if (columnName.IsWildcard) {
				throw new ArgumentException("Wilcard column names are not allowed for table column definitions", "columnName");
			}
			this.columnName = columnName;
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}
	}
}