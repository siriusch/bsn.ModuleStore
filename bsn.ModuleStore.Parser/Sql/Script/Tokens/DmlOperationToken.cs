using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public abstract class DmlOperationToken: SqlToken {
		public abstract DmlOperation Operation {
			get;
		}
	}
}