using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class UpdateMode: SqlToken, IScriptable {
		public abstract UpdateModeKind Kind {
			get;
		}

		public abstract void WriteTo(TextWriter writer);
	}
}