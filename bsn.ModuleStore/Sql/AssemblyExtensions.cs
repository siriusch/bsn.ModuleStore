using System;
using System.Collections.Generic;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public static class AssemblyExtensions {
		public static bool DependsOnTables(this IScriptableStatement that, ICollection<string> tableNames) {
			switch (that) {
			case ITableBound tableBound:
				var tableName = tableBound.TableName;
				return tableName != null && tableNames.Contains(tableName.Name.Value);
			default:
				return false;
			}
		}

		public static bool IsTableUniqueConstraintOfTables(this IInstallStatement that, ICollection<string> tableNames) {
			switch (that) {
			case AlterTableAddConstraintFragment constraint:
				return (tableNames.Contains(constraint.Owner.ObjectName)) && (constraint.Constraint is TableUniqueConstraintBase);
			default:
				return false;
			}
		}
	}
}
