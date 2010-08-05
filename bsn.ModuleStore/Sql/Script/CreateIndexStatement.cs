using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class CreateIndexStatement: SqlCreateStatement {
		private readonly IndexName indexName;
		private readonly TableName tableName;
		private readonly Sequence<IndexOption> indexOptions;

		protected CreateIndexStatement(IndexName indexName, TableName tableName, Optional<Sequence<IndexOption>> indexOptions) {
			if (indexName == null) {
				throw new ArgumentNullException("indexName");
			}
			if (tableName == null) {
				throw new ArgumentNullException("tableName");
			}
			this.indexName = indexName;
			this.tableName = tableName;
			this.indexOptions = indexOptions;
		}
	}
}
