using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnWildcardItem: ColumnItem {
		private readonly Qualified<ColumnName> columnWildcard;

		[Rule("<ColumnItem> ::= <ColumnWildQualified>")]
		public ColumnWildcardItem(Qualified<ColumnName> columnWildcard) {
			Debug.Assert(columnWildcard != null);
			this.columnWildcard = columnWildcard;
		}

		public Qualified<ColumnName> ColumnWildcard {
			get {
				return columnWildcard;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(columnWildcard, WhitespacePadding.None);
		}
	}
}