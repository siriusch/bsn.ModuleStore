using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class CreateStatement: DdlStatement {
		public abstract string ObjectName {
			get;
		}
	}
}