using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class ConstraintIndex: SqlScriptableToken, IOptional {
		public abstract bool HasValue {
			get;
		}
	}
}