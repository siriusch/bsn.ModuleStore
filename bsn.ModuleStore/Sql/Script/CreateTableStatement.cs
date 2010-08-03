using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateTableStatement: SqlCreateStatement {
		private readonly TableDefinitionGroup definition;
		private readonly TableName tableName;

		[Rule("<CreateTableStatement> ::= CREATE TABLE <TableName> <TableDefinitionGroup>", ConstructorParameterMapping = new[] {2, 3})]
		public CreateTableStatement(TableName tableName, TableDefinitionGroup definition) {
			this.tableName = tableName;
			this.definition = definition;
		}
	}
}