using System;
using System.Collections.Generic;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class Statement: CommentContainerToken {
		public IEnumerable<SqlName> GetReferencedObjectNames() {
			return GetInnerTokens<IQualifiedName<SchemaName>>((qn, scope) => (!qn.LockedOverride) && qn.IsOverridden, null).Select(qn => qn.Name).Distinct();
		}
	}
}