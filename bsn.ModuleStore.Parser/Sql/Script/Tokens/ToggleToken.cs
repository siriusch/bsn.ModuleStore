using System;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public abstract class ToggleToken: SqlScriptableToken {
		public abstract bool On {
			get;
		}
	}
}