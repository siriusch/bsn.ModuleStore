using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TableCheckToken: SqlToken {
		[Rule("<TableCheck> ::=")]
		public TableCheckToken() {}

		public virtual TableCheck TableCheck {
			get {
				return TableCheck.Default;
			}
		}
	}
}