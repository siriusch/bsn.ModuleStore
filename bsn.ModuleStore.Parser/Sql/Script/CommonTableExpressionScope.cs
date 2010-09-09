using System;
using System.Collections.Generic;

namespace bsn.ModuleStore.Sql.Script {
	internal class CommonTableExpressionScope {
		private readonly HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		private readonly CommonTableExpressionScope parent;

		public CommonTableExpressionScope(CommonTableExpressionScope parent) {
			this.parent = parent;
		}

		public void AddName(string name) {
			names.Add(name);
		}

		public bool ContainsName(string name) {
			if (names.Contains(name)) {
				return true;
			}
			if (parent != null) {
				return parent.ContainsName(name);
			}
			return false;
		}
	}
}
