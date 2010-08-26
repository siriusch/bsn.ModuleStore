﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class CreateIndexStatement: CreateStatement {
		private readonly IndexName indexName;
		private readonly List<IndexOption> indexOptions;
		private readonly Qualified<SchemaName, TableName> tableName;

		protected CreateIndexStatement(IndexName indexName, Qualified<SchemaName, TableName> tableName, Optional<Sequence<IndexOption>> indexOptions) {
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

		public override sealed string ObjectSchema {
			get {
				return tableName.IsQualified ? tableName.Qualification.Value : string.Empty;
			}
		}

		public List<IndexOption> IndexOptions {
			get {
				return indexOptions;
			}
		}

		public override sealed ObjectCategory ObjectCategory {
			get {
				return ObjectCategory.Index;
			}
		}

		public override string ObjectName {
			get {
				return indexName.Value;
			}
		}

		public Qualified<SchemaName, TableName> TableName {
			get {
				return tableName;
			}
		}
	}
}