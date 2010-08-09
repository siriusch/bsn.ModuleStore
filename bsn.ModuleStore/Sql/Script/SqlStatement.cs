using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlStatement: SqlToken, IScriptable {
		public abstract void WriteTo(TextWriter writer);
	}
}