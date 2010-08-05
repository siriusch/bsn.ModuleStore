using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ColumnExpressionItem: ColumnItem {
		[Rule("<ColumnItem> ::= <ColumnWildQualified>")]
		public ColumnExpressionItem(Qualified<ColumnName> columnWildcard) {}
	}
}