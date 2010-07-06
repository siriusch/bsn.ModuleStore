using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public class TypedColumnDefinition: ColumnDefinition {
		private readonly ColumnType columnType;

		public TypedColumnDefinition(ColumnName columnName, ColumnType columnType): base(columnName) {
			if (columnType == null) {
				throw new ArgumentNullException("columnType");
			}
			this.columnType = columnType;
		}

		public override void WriteTo(TextWriter writer) {
			ColumnName.WriteTo(writer);
			writer.Write(' ');
			columnType.WriteTo(writer);
		}
	}
}