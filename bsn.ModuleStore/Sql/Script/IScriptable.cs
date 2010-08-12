using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public interface IScriptable {
		void WriteTo(SqlWriter writer);
	}
}