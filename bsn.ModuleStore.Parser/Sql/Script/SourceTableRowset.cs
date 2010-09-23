using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceTableRowset: SourceRowset {
		private readonly Qualified<SchemaName, TableName> tableName;

		[Rule("<SourceRowset> ::= <TableNameQualified> <RowsetAlias>")]
		public SourceTableRowset(Qualified<SchemaName, TableName> tableName, RowsetAlias rowsetAlias): base(rowsetAlias) {
			Debug.Assert(tableName != null);
			this.tableName = tableName;
		}

		public Qualified<SchemaName, TableName> TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(tableName, WhitespacePadding.None);
			base.WriteTo(writer);
		}
	}
}