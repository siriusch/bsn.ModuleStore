using System;
using System.Collections.Generic;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public static class AssemblyExtensions {
		public static bool IsTableUniqueConstraintOfTables(this IInstallStatement statement, ICollection<string> tableNames) {
			AlterTableAddConstraintFragment constraint = statement as AlterTableAddConstraintFragment;
			if (constraint != null) {
				return (tableNames.Contains(constraint.Owner.ObjectName)) && (constraint.Constraint is TableUniqueConstraintBase);
			}
			return false;
		}
	}
}
