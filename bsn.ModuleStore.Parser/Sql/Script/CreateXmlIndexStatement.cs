using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateXmlIndexStatement: CreateIndexStatement {
		private readonly ColumnName columnName;
		private readonly IndexUsing indexUsing;

		[Rule("<CreateIndexStatement> ::= ~CREATE ~XML ~INDEX <IndexName> ~ON <TableNameQualified> ~'(' <ColumnName> ~')' <IndexUsing> <IndexOptionGroup>")]
		public CreateXmlIndexStatement(IndexName indexName, Qualified<SchemaName, TableName> tableName, ColumnName columnName, IndexUsing indexUsing, Optional<Sequence<IndexOption>> indexOptions): base(indexName, tableName, indexOptions) {
			this.columnName = columnName;
			this.indexUsing = indexUsing;
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}

		public IndexUsing IndexUsing {
			get {
				return indexUsing;
			}
		}

		public virtual bool Primary {
			get {
				return false;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("CREATE ");
			if (Primary) {
				writer.Write("PRIMARY ");
			}
			writer.Write("XML INDEX ");
			writer.WriteScript(IndexName, WhitespacePadding.None);
			writer.Write(" ON ");
			writer.WriteScript(TableName, WhitespacePadding.None);
			writer.Write(" (");
			writer.WriteScript(columnName, WhitespacePadding.None);
			writer.Write(") ");
			writer.WriteScript(indexUsing, WhitespacePadding.None);
			writer.WriteIndexOptions(IndexOptions);
		}
	}
}