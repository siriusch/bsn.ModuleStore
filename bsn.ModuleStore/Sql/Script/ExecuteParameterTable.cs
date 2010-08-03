using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ExecuteParameterTable: ExecuteParameter {
		private readonly Qualified<TableName> tableName;

		[Rule("<ExecuteParameter> ::= <TableNameQualified>")]
		public ExecuteParameterTable(Qualified<TableName> tableName) {
			this.tableName = tableName;
		}
	}
}