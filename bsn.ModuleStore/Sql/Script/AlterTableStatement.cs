using System;
using System.IO;
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

		public TableName TableName {
			get {
				return tableName;
			}
		}

		public virtual void WriteTo(TextWriter writer) {
			writer.Write("ALTER TABLE ");
			writer.WriteScript(tableName);
			writer.Write(' ');
		}
	}
}
