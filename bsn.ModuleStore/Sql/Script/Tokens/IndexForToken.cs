using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public abstract class IndexForToken: SqlToken {
		public abstract IndexFor IndexFor {
			get;
		}
	}
}