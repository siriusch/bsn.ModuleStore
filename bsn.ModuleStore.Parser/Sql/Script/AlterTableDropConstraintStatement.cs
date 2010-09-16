using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class AlterTableDropConstraintStatement: AlterTableStatement {
		private readonly ConstraintName constraintName;

		[Rule("<AlterTableStatement> ::= ~ALTER ~TABLE <TableNameQualified> ~DROP <ConstraintName>")]
		[Rule("<AlterTableStatement> ::= ~ALTER ~TABLE <TableNameQualified> ~DROP ~CONSTRAINT <ConstraintName>")]
		public AlterTableDropConstraintStatement(Qualified<SchemaName, TableName> tableName, ConstraintName constraintName): base(tableName) {
			Debug.Assert(constraintName != null);
			this.constraintName = constraintName;
		}

		public ConstraintName ConstraintName {
			get {
				return constraintName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("DROP CONSTRAINT ");
			writer.WriteScript(constraintName, WhitespacePadding.None);
		}
	}
}