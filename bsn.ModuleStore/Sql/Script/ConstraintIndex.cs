using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class ConstraintIndex: SqlToken, IScriptable, IOptional {
		public abstract bool HasValue {
			get;
		}

		public abstract void WriteTo(TextWriter writer);
	}
}