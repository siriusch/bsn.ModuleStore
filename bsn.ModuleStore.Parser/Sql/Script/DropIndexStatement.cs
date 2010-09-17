using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DropIndexStatement: DropStatement {
		private readonly IndexName indexName;
		private readonly List<IndexOption> indexOptions;
		private readonly Qualified<SchemaName, TableName> tableName;

		[Rule("<DropIndexStatement> ::= ~DROP ~INDEX <IndexName> ~ON <TableNameQualified> <IndexOptionGroup>")]
		public DropIndexStatement(IndexName indexName, Qualified<SchemaName, TableName> tableName, Optional<Sequence<IndexOption>> indexOptions) {
			Debug.Assert(indexName != null);
			Debug.Assert(tableName != null);
			this.indexName = indexName;
			this.tableName = tableName;
			this.indexOptions = indexOptions.ToList();
		}

		public IndexName IndexName {
			get {
				return indexName;
			}
		}

		public IEnumerable<IndexOption> IndexOptions {
			get {
				return indexOptions;
			}
		}

		public Qualified<SchemaName, TableName> TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("DROP INDEX ");
			writer.WriteScript(indexName, WhitespacePadding.None);
			writer.Write(" ON ");
			writer.WriteScript(tableName, WhitespacePadding.None);
			writer.WriteIndexOptions(indexOptions);
		}
	}
}