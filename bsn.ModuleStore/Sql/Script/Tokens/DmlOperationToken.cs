using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public abstract class DmlOperationToken: SqlToken {
		protected DmlOperationToken() {}

		public abstract DmlOperation Operation {
			get;
		}
	}
}
