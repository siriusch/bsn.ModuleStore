using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public class TableWithNocheckToken: TableCheckToken {
		[Rule("<TableCheck> ::= ~WITH ~NOCHECK")]
		public TableWithNocheckToken() {}

		public override TableCheck TableCheck {
			get {
				return TableCheck.Nocheck;
			}
		}
	}
}