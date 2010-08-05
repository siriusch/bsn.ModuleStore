using System;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ColumnNullableConstraint: ColumnConstraint {
		private readonly bool nullable;

		[Rule("<ColumnConstraint> ::= NULL")]
		[Rule("<ColumnConstraint> ::= NOT NULL", AllowTruncationForConstructor = true)]
		public ColumnNullableConstraint(IToken token) {
			nullable = token.Symbol.Name.Equals("NULL", StringComparison.Ordinal);
		}
	}
}