using System;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public abstract class ToggleToken: SqlToken, IScriptable {
		public abstract bool On {
			get;
		}

		public abstract void WriteTo(SqlWriter writer);
	}
}