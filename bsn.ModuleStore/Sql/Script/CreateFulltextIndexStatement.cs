using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateFulltextIndexStatement: CreateStatement {
		private readonly FulltextChangeTracking changeTracking;
		private readonly List<FulltextColumn> columns;
		private readonly IndexName indexName;
		private readonly Qualified<SchemaName, TableName> tableName;

		[Rule("<CreateFulltextStatement> ::= CREATE FULLTEXT_INDEX ON TABLE <TableNameQualified> <FulltextColumnGroup> KEY INDEX <IndexName> <FulltextChangeTracking>", ConstructorParameterMapping = new[] {4, 5, 8, 9})]
		public CreateFulltextIndexStatement(Qualified<SchemaName, TableName> tableName, Optional<Sequence<FulltextColumn>> columns, IndexName indexName, FulltextChangeTrackingToken changeTracking) {
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

		public override string ObjectSchema {
			get {
				return tableName.IsQualified ? tableName.Qualification.Value : string.Empty;
			}
		}

		public Qualified<SchemaName, TableName> TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CREATE FULLTEXT INDEX ON TABLE ");
			writer.WriteScript(tableName, WhitespacePadding.None);
			if (columns.Count > 0) {
				writer.Write(" (");
				writer.WriteScriptSequence(columns, WhitespacePadding.None, ", ");
				writer.Write(")");
			}
			writer.Write(" KEY INDEX ");
			writer.WriteScript(indexName, WhitespacePadding.None);
			writer.WriteEnum(changeTracking, WhitespacePadding.SpaceBefore);
		}
	}
}