using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public abstract class ToggleToken: SqlToken {
		protected ToggleToken() {}

		public abstract bool On {
			get;
		}
	}
}