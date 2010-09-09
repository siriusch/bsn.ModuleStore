using System;
using System.Collections.Generic;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class Statement: SqlScriptableToken {
		public IEnumerable<SqlName> GetReferencedObjectNames() {
			return GetInnerTokens<IQualifiedName<SchemaName>>((qn, scope) => qn.IsOverridden).Select(qn => qn.Name).Distinct();
		}
	}
}