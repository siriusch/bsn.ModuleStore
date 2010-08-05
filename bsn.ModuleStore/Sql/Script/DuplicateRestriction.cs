using System;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class DuplicateRestriction: SqlToken {
		private readonly bool distinct;

		[Rule("<Restriction> ::=")]
		public DuplicateRestriction(): this(null) {}

		[Rule("<Restriction> ::= ALL")]
		[Rule("<Restriction> ::= DISTINCT")]
		public DuplicateRestriction(IToken token) {
			distinct = (token != null) && token.Symbol.Name.Equals("DISTINCT", StringComparison.Ordinal);
		}

		public bool Distinct {
			get {
				return distinct;
			}
		}
	}
}