using System;
using System.IO;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class AlterTableStatement: Statement {
		private readonly TableName tableName;

		protected AlterTableStatement(TableName tableName) {
			if (tableName == null) {
				throw new ArgumentNullException("tableName");
			}
			this.tableName = tableName;
		}

		public TableName TableName {
			get {
				return tableName;
			}
		}

		public abstract void ApplyTo(CreateTableStatement createTable);

		public override void WriteTo(TextWriter writer) {
			writer.Write("ALTER TABLE ");
			writer.WriteScript(tableName);
			writer.Write(' ');
		}
	}
}