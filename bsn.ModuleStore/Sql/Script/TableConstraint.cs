using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class TableConstraint: TableDefinition {
		private readonly ConstraintName constraintName;

		protected TableConstraint(ConstraintName constraintName) {
			this.constraintName = constraintName;
		}

		public ConstraintName ConstraintName {
			get {
				return constraintName;
			}
		}

		public override void WriteTo(TextWriter writer) {
			if (constraintName != null) {
				writer.Write("CONSTRAINT ");
				writer.WriteScript(constraintName);
				writer.Write(' ');
			}
		}
	}
}