using System;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public abstract class DuplicateRestrictionToken: SqlToken {
		public abstract bool Distinct {
			get;
		}
	}
}