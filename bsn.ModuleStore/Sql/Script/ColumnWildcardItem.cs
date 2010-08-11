using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnWildcardItem: ColumnItem {
		private readonly Qualified<ColumnName> columnWildcard;

		[Rule("<ColumnItem> ::= <ColumnWildQualified>")]
		public ColumnWildcardItem(Qualified<ColumnName> columnWildcard) {
			if (columnWildcard == null) {
				throw new ArgumentNullException("columnWildcard");
			}
			this.columnWildcard = columnWildcard;
		}

		public Qualified<ColumnName> ColumnWildcard {
			get {
				return columnWildcard;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteScript(columnWildcard);
		}
	}
}