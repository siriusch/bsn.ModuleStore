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

		public virtual void ApplyTo(CreateTableStatement createTable) {
			throw new NotImplementedException();
		}

		public TableName TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("ALTER TABLE ");
			writer.WriteScript(tableName);
			writer.Write(' ');
		}
	}
}
