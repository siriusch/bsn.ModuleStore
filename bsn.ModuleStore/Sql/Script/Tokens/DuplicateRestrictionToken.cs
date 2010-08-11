using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public abstract class DuplicateRestrictionToken: SqlToken {
		public DuplicateRestrictionToken(): base() {}

		public abstract bool Distinct {
			get;
		}
	}
}