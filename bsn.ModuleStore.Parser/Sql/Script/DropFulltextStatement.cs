using System;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DropFulltextStatement: DropStatement {
		private readonly Qualified<SchemaName, TableName> tableName;

		[Rule("<DropFulltextStatement> ::= ~DROP ~FULLTEXT ~INDEX ~ON <TableNameQualified>")]
		public DropFulltextStatement(Qualified<SchemaName, TableName> tableName) {
			Debug.Assert(tableName != null);
			this.tableName = tableName;
		}

		public Qualified<SchemaName, TableName> TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("DROP FULLTEXT INDEX ON ");
			writer.WriteScript(tableName, WhitespacePadding.None);
		}
	}
}