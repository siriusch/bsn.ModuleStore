using System;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public abstract class DdlOperationToken: SqlToken {
		public abstract DdlOperation Operation {
			get;
		}
	}
}