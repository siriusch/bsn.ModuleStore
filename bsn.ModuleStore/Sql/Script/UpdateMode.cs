using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class UpdateMode: SqlScriptableToken {
		public abstract UpdateModeKind Kind {
			get;
		}
	}
}