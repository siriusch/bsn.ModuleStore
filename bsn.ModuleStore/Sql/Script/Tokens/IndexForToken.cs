using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public abstract class IndexForToken {
		protected IndexForToken() {}

		public abstract IndexFor IndexFor {
			get;
		}
	}
}
