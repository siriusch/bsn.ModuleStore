using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class AlterTableColumnDefinitionStatement: AlterTableColumnStatement {
		private readonly ColumnDefinition definition;

		[Rule("<AlterTableStatement> ::= ~ALTER ~TABLE <TableNameQualified> ~ALTER ~COLUMN <ColumnName> <ColumnDefinition>")]
		public AlterTableColumnDefinitionStatement(Qualified<SchemaName, TableName> tableName, ColumnName columnName, ColumnDefinition definition): base(tableName, columnName) {
			Debug.Assert(definition != null);
			this.definition = definition;
		}

		public ColumnDefinition Definition {
			get {
				return definition;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.WriteScript(definition, WhitespacePadding.None);
		}
	}
}