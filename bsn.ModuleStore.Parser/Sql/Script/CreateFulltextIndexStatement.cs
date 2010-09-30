using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateFulltextIndexStatement: CreateStatement {
		private readonly FulltextChangeTracking changeTracking;
		private readonly List<FulltextColumn> columns;
		private readonly IndexName indexName;
		private readonly Qualified<SchemaName, TableName> tableName;

		[Rule("<CreateFulltextStatement> ::= ~CREATE ~FULLTEXT ~INDEX ~ON ~TABLE <TableNameQualified> <FulltextColumnGroup> ~KEY ~INDEX <IndexName> <FulltextChangeTracking>")]
		public CreateFulltextIndexStatement(Qualified<SchemaName, TableName> tableName, Optional<Sequence<FulltextColumn>> columns, IndexName indexName, FulltextChangeTracking changeTracking) {
			Debug.Assert(tableName != null);
			Debug.Assert(indexName != null);
			Debug.Assert(changeTracking != null);
			this.tableName = tableName;
			this.columns = columns.ToList();
			this.indexName = indexName;
			this.changeTracking = changeTracking;
		}

		public FulltextChangeTracking ChangeTracking {
			get {
				return changeTracking;
			}
		}

		public IEnumerable<FulltextColumn> Columns {
			get {
				return columns;
			}
		}

		public IndexName IndexName {
			get {
				return indexName;
			}
		}

		public override ObjectCategory ObjectCategory {
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

		public override DropStatement CreateDropStatement() {
			return new DropFulltextStatement(tableName);
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("CREATE FULLTEXT INDEX ON TABLE ");
			writer.WriteScript(tableName, WhitespacePadding.None);
			if (columns.Count > 0) {
				writer.Write(" (");
				writer.WriteScriptSequence(columns, WhitespacePadding.None, ", ");
				writer.Write(')');
			}
			writer.Write(" KEY INDEX ");
			writer.WriteScript(indexName, WhitespacePadding.None);
			writer.WriteScript(changeTracking, WhitespacePadding.SpaceBefore);
		}

		protected override string GetObjectSchema() {
			return tableName.IsQualified ? tableName.Qualification.Value : string.Empty;
		}
	}
}