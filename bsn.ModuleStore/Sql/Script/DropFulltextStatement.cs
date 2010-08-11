using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DropFulltextStatement: DropStatement {
		private readonly TableName tableName;

		[Rule("<DropFulltextStatement> ::= DROP FULLTEXT_INDEX ON <TableName>", ConstructorParameterMapping = new[] {3})]
		public DropFulltextStatement(TableName tableName) {
			Debug.Assert(tableName != null);
			this.tableName = tableName;
		}

		public TableName TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("DROP FULLTEXT INDEX ON ");
			writer.WriteScript(tableName);
		}
	}
}