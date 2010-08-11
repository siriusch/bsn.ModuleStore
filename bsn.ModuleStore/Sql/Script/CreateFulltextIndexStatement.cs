using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateFulltextIndexStatement: CreateStatement {
		private readonly FulltextChangeTracking changeTracking;
		private readonly List<FulltextColumn> columns;
		private readonly IndexName indexName;
		private readonly TableName tableName;

		[Rule("<CreateFulltextStatement> ::= CREATE FULLTEXT_INDEX ON TABLE <TableName> <FulltextColumnGroup> KEY INDEX <IndexName> <FulltextChangeTracking>", ConstructorParameterMapping = new[] {4, 5, 8, 9})]
		public CreateFulltextIndexStatement(TableName tableName, Optional<Sequence<FulltextColumn>> columns, IndexName indexName, FulltextChangeTrackingToken changeTracking) {
			Debug.Assert(tableName != null);
			Debug.Assert(indexName != null);
			Debug.Assert(changeTracking != null);
			this.tableName = tableName;
			this.columns = columns.ToList();
			this.indexName = indexName;
			this.changeTracking = changeTracking.FulltextChangeTracking;
		}

		public FulltextChangeTracking ChangeTracking {
			get {
				return changeTracking;
			}
		}

		public List<FulltextColumn> Columns {
			get {
				return columns;
			}
		}

		public IndexName IndexName {
			get {
				return indexName;
			}
		}

		public TableName TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("CREATE FULLTEXT INDEX ON TABLE ");
			writer.WriteScript(tableName);
			if (columns.Count > 0) {
				writer.Write(" (");
				writer.WriteSequence(columns, null, ", ", null);
				writer.Write(")");
			}
			writer.Write(" KEY INDEX ");
			writer.WriteScript(indexName);
			writer.WriteValue(changeTracking, " ", null);
		}
	}
}