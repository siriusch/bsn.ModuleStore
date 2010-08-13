using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnWildcardItem: ColumnItem {
		private readonly Qualified<SqlName, ColumnName> columnWildcard;

		[Rule("<ColumnItem> ::= <ColumnWildQualified>")]
		public ColumnWildcardItem(Qualified<SqlName, ColumnName> columnWildcard) {
			Debug.Assert(columnWildcard != null);
			this.columnWildcard = columnWildcard;
		}

		public Qualified<SqlName, ColumnName> ColumnWildcard {
			get {
				return columnWildcard;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(columnWildcard, WhitespacePadding.None);
		}
	}
}