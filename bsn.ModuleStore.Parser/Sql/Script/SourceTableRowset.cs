using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceTableRowset: SourceRowset {
		private readonly Qualified<SchemaName, TableName> tableName;
		private readonly TableHintGroup tableHints;

		[Rule("<SourceRowset> ::= <TableNameQualified> <RowsetAlias> <TableHintGroup>")]
		public SourceTableRowset(Qualified<SchemaName, TableName> tableName, RowsetAlias rowsetAlias, TableHintGroup tableHints): base(rowsetAlias) {
			Debug.Assert(tableName != null);
			this.tableName = tableName;
			this.tableHints = tableHints;
		}

		public Qualified<SchemaName, TableName> TableName {
			get {
				return tableName;
			}
		}

		public TableHintGroup TableHints {
			get {
				return tableHints;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(tableName, WhitespacePadding.None);
			base.WriteTo(writer);
			writer.WriteScript(tableHints, WhitespacePadding.SpaceBefore);
		}
	}
}