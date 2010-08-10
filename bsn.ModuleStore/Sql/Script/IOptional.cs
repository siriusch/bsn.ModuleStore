using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public interface IOptional: IScriptable {
		bool HasValue {
			get;
		}
	}
}