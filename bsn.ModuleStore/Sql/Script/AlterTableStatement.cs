using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class AlterTableStatement: SqlStatement {
		private readonly TableName tableName;

		protected AlterTableStatement(TableName tableName) {
			if (tableName == null) {
				throw new ArgumentNullException("tableName");
			}
			this.tableName = tableName;
		}

		public abstract void ApplyTo(CreateTableStatement createTable);

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("ALTER TABLE ");
			tableName.WriteTo(writer);
			writer.Write(' ');
		}

		public TableName TableName {
			get {
				return tableName;
			}
		}
	}
}