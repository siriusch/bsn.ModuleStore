using System;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateFulltextIndexStatement: SqlCreateStatement {
		private readonly FulltextChangeTracking changeTracking;
		private readonly Sequence<FulltextColumn> columns;
		private readonly IndexName indexName;
		private readonly TableName tableName;

		[Rule("<CreateFulltextStatement> ::= CREATE FULLTEXT_INDEX ON TABLE <TableName> <FulltextColumnGroup> KEY INDEX <IndexName> <OptionalFulltextChangeTracking>", ConstructorParameterMapping = new[] {4, 5, 8, 9})]
		public CreateFulltextIndexStatement(TableName tableName, Optional<Sequence<FulltextColumn>> columns, IndexName indexName, Optional<FulltextChangeTracking> changeTracking) {
			if (tableName == null) {
				throw new ArgumentNullException("tableName");
			}
			if (indexName == null) {
				throw new ArgumentNullException("indexName");
			}
			this.tableName = tableName;
			this.columns = columns;
			this.indexName = indexName;
			this.changeTracking = changeTracking;
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("CREATE FULLTEXT INDEX ON TABLE ");
			tableName.WriteTo(writer);
			if (columns != null) {
				string separator = " (";
				foreach (FulltextColumn column in columns) {
					writer.Write(separator);
					column.WriteTo(writer);
					separator = ", ";
				}
				writer.Write(")");
			}
			writer.Write(" KEY INDEX ");
			indexName.WriteTo(writer);
			if (changeTracking != null) {
				changeTracking.WriteTo(writer);
			}
		}
	}
}