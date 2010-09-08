using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public class TableWithCheckToken: TableCheckToken {
		[Rule("<TableCheck> ::= WITH CHECK", AllowTruncationForConstructor = true)]
		public TableWithCheckToken() {}

		public override TableCheck TableCheck {
			get {
				return TableCheck.Check;
			}
		}
	}
}