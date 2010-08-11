using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlComputable: SqlToken, IScriptable {
		public abstract void WriteTo(SqlWriter writer);
	}
}