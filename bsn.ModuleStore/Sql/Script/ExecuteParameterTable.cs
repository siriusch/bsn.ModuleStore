using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExecuteParameterTable: ExecuteParameter {
		private readonly Qualified<SchemaName, TableName> tableName;

		[Rule("<ExecuteParameter> ::= <TableNameQualified>")]
		public ExecuteParameterTable(Qualified<SchemaName, TableName> tableName) {
			Debug.Assert(tableName != null);
			this.tableName = tableName;
		}

		public Qualified<SchemaName, TableName> TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(tableName, WhitespacePadding.None);
		}
	}
}