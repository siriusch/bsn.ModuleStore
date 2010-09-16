using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class AlterTableDropColumnStatement: AlterTableStatement {
		private readonly ColumnName columnName;

		[Rule("<AlterTableStatement> ::= ~ALTER ~TABLE <TableNameQualified> ~DROP ~COLUMN <ColumnName>")]
		public AlterTableDropColumnStatement(Qualified<SchemaName, TableName> tableName, ColumnName columnName): base(tableName) {
			Debug.Assert(columnName != null);
			this.columnName = columnName;
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("DROP COLUMN ");
			writer.WriteScript(columnName, WhitespacePadding.None);
		}
	}
}