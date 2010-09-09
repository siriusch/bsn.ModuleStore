using System;

namespace bsn.ModuleStore.Sql.Script {
	public interface IQualified<TQ> where TQ: SqlName {
		TQ Qualification {
			get;
		}
	}
}
