using System;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DropTableStatement: DropStatement {
		private readonly Qualified<SchemaName, TableName> tableName;

		[Rule("<DropTableStatement> ::= ~DROP ~TABLE <TableNameQualified>")]
		public DropTableStatement(Qualified<SchemaName, TableName> tableName) {
			Debug.Assert(tableName != null);
			this.tableName = tableName;
		}

		public Qualified<SchemaName, TableName> TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("DROP TABLE ");
			writer.WriteScript(tableName, WhitespacePadding.None);
		}
	}
}