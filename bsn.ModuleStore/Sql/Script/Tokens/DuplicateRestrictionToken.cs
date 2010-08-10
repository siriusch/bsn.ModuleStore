using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public class DuplicateRestrictionToken: SqlToken {
		[Rule("<Restriction> ::=")]
		public DuplicateRestrictionToken(): base() {}

		public virtual bool? Distinct {
			get {
				return null;
			}
		}
	}
}