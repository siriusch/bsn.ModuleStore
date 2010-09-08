using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public interface IOptional {
		bool HasValue {
			get;
		}
	}
}