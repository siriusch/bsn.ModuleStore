using System;
using System.IO;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public interface IScriptable {
		void WriteTo(SqlWriter writer);
	}
}