using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class DropFulltextStatement: SqlStatement {
		private readonly TableName tableName;

		[Rule("<DropFulltextStatement> ::= DROP FULLTEXT_INDEX ON <TableName>", ConstructorParameterMapping = new[] {3})]
		public DropFulltextStatement(TableName tableName) {
			if (tableName == null) {
				throw new ArgumentNullException("tableName");
			}
			this.tableName = tableName;
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("DROP FULLTEXT INDEX ON ");
			tableName.WriteTo(writer);
		}
	}
}
