using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class AlterTableAddStatement: AlterTableStatement {
		private readonly TableCheck check;
		private readonly Sequence<TableDefinition> definitions;

		[Rule("<AlterTableStatement> ::= ALTER TABLE <TableName> <TableCheck> ADD <TableDefinitionList>", ConstructorParameterMapping = new[] {2, 3, 5})]
		public AlterTableAddStatement(TableName tableName, TableCheck check, Sequence<TableDefinition> definitions): base(tableName) {
			if (check == null) {
				throw new ArgumentNullException("check");
			}
			if (definitions == null) {
				throw new ArgumentNullException("definitions");
			}
			this.check = check;
			this.definitions = definitions;
		}
	}
}