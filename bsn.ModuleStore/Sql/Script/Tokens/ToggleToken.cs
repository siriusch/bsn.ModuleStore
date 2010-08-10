using System;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public abstract class ToggleToken: SqlToken {
		public abstract bool On {
			get;
		}
	}
}