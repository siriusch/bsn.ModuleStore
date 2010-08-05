using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class DropTableStatement: SqlDropStatement {
		private readonly TableName tableName;

		[Rule("<DropTableStatement> ::= DROP TABLE <TableName>", ConstructorParameterMapping = new[] {2})]
		public DropTableStatement(TableName tableName) {
			if (tableName == null) {
				throw new ArgumentNullException("tableName");
			}
			this.tableName = tableName;
		}
	}
}