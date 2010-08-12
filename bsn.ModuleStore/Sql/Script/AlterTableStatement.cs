using System;
using System.Diagnostics;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class AlterTableStatement: Statement {
		private readonly TableName tableName;

		protected AlterTableStatement(TableName tableName) {
			Debug.Assert(tableName != null);
			this.tableName = tableName;
		}

		public TableName TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("ALTER TABLE ");
			writer.WriteScript(tableName, WhitespacePadding.SpaceAfter);
		}
	}
}