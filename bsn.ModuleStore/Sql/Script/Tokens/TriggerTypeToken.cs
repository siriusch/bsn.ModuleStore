using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public abstract class TriggerTypeToken: SqlToken {
		public abstract TriggerType TriggerType {
			get;
		}
	}
}