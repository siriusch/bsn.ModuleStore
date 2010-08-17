using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public interface IQualifiedName<TQ> where TQ: SqlName {
		bool IsQualified {
			get;
		}

		SqlName Name {
			get;
		}

		TQ Qualification {
			get;
			set;
		}
	}
}