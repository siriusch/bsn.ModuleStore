using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class AlterTableDropColumnStatement: AlterTableStatement {
		private readonly ColumnName columnName;

		[Rule("<AlterTableStatement> ::= ALTER TABLE <TableName> DROP COLUMN <ColumnName>", ConstructorParameterMapping = new[] {2, 5})]
		public AlterTableDropColumnStatement(TableName tableName, ColumnName columnName): base(tableName) {
			if (columnName == null) {
				throw new ArgumentNullException("columnName");
			}
			this.columnName = columnName;
		}

		public override void ApplyTo(CreateTableStatement createTable) {
			throw new NotImplementedException();
		}
	}
}