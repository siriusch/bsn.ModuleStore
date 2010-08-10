using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExecuteParameterTable: ExecuteParameter {
		private readonly Qualified<TableName> tableName;

		[Rule("<ExecuteParameter> ::= <TableNameQualified>")]
		public ExecuteParameterTable(Qualified<TableName> tableName) {
			if (tableName == null) {
				throw new ArgumentNullException("tableName");
			}
			this.tableName = tableName;
		}

		public Qualified<TableName> TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteScript(tableName);
		}
	}
}