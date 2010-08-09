using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class ColumnConstraint: SqlToken, IScriptable {
		public abstract void WriteTo(TextWriter writer);
	}
}