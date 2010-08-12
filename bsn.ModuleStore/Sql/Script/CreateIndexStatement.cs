using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class CreateIndexStatement: CreateStatement {
		private readonly IndexName indexName;
		private readonly List<IndexOption> indexOptions;
		private readonly TableName tableName;

		protected CreateIndexStatement(IndexName indexName, TableName tableName, Optional<Sequence<IndexOption>> indexOptions) {
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

		public List<IndexOption> IndexOptions {
			get {
				return indexOptions;
			}
		}

		public TableName TableName {
			get {
				return tableName;
			}
		}
	}
}