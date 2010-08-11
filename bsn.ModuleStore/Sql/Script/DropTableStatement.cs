using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DropTableStatement: DropStatement {
		private readonly TableName tableName;

		[Rule("<DropTableStatement> ::= DROP TABLE <TableName>", ConstructorParameterMapping = new[] {2})]
		public DropTableStatement(TableName tableName) {
			Debug.Assert(tableName != null);
			this.tableName = tableName;
		}

		public TableName TableName {
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