using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ColumnWildcardItem: ColumnItem {
		[Rule("<ColumnItem> ::= <Expression> <OptionalAlias>")]
		public ColumnWildcardItem(Expression expression, Optional<AliasName> aliasName) {}
	}
}