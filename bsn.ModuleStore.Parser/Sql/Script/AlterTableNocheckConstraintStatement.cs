using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class AlterTableNocheckConstraintStatement: AlterTableCheckConstraintStatementBase {
		[Rule("<AlterTableStatement> ::= ~ALTER ~TABLE <TableNameQualified> <TableCheck> ~NOCHECK ~CONSTRAINT <ConstraintName>")]
		public AlterTableNocheckConstraintStatement(Qualified<SchemaName, TableName> tableName, TableCheckToken tableCheck, ConstraintName constraintName): base(tableName, tableCheck, constraintName) {}

		[Rule("<AlterTableStatement> ::= ~ALTER ~TABLE <TableNameQualified> <TableCheck> ~NOCHECK ~CONSTRAINT ~ALL")]
		public AlterTableNocheckConstraintStatement(Qualified<SchemaName, TableName> tableName, TableCheckToken tableCheck): this(tableName, tableCheck, null) {}

		protected override void WriteCheckOperation(SqlWriter writer) {
			writer.Write("NOCHECK");
		}
	}
}