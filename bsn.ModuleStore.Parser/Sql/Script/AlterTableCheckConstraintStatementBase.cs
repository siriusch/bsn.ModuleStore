using System;
using System.Diagnostics;

using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class AlterTableCheckConstraintStatementBase: AlterTableStatement {
		private readonly ConstraintName constraintName;
		private readonly TableCheck tableCheck;

		protected AlterTableCheckConstraintStatementBase(Qualified<SchemaName, TableName> tableName, TableCheckToken tableCheck, ConstraintName constraintName): base(tableName) {
			Debug.Assert(constraintName != null);
			this.tableCheck = tableCheck.TableCheck;
			this.constraintName = constraintName;
		}

		public ConstraintName ConstraintName {
			get {
				return constraintName;
			}
		}

		public TableCheck TableCheck {
			get {
				return tableCheck;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.WriteEnum(tableCheck, WhitespacePadding.SpaceAfter);
			WriteCheckOperation(writer);
			writer.Write(" CONSTRAINT ");
			if (constraintName != null) {
				writer.WriteScript(constraintName, WhitespacePadding.None);
			} else {
				writer.Write("ALL");
			}
		}

		protected abstract void WriteCheckOperation(SqlWriter writer);
	}
}