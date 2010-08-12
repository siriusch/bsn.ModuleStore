using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class AlterTableCheckConstraintStatement: AlterTableCheckConstraintStatementBase {
		[Rule("<AlterTableStatement> ::= ALTER TABLE <TableName> <TableCheck> CHECK CONSTRAINT <ConstraintName>", ConstructorParameterMapping = new[] {2, 3, 6})]
		public AlterTableCheckConstraintStatement(TableName tableName, TableCheckToken tableCheck, ConstraintName constraintName): base(tableName, tableCheck, constraintName) {}

		[Rule("<AlterTableStatement> ::= ALTER TABLE <TableName> <TableCheck> CHECK CONSTRAINT ALL", ConstructorParameterMapping = new[] {2, 3})]
		public AlterTableCheckConstraintStatement(TableName tableName, TableCheckToken tableCheck): this(tableName, tableCheck, null) {}

		protected override void WriteCheckOperation(SqlWriter writer) {
			writer.Write("CHECK");
		}
	}
}