using System;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DropTableStatement: DropStatement {
		private readonly TableName tableName;

		[Rule("<DropTableStatement> ::= DROP TABLE <TableName>", ConstructorParameterMapping = new[] {2})]
		public DropTableStatement(TableName tableName) {
			if (tableName == null) {
				throw new ArgumentNullException("tableName");
			}
			this.tableName = tableName;
		}

		public TableName TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("DROP TABLE ");
			writer.WriteScript(tableName);
		}
	}
}