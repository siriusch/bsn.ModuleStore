using System;
using System.Collections.Generic;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public static class AssemblyExtensions {
		public static bool DependsOnTables(this IScriptableStatement that, ICollection<string> tableNames) {
			ITableBound tableBound = that as ITableBound;
			if (tableBound != null) {
				Qualified<SchemaName, TableName> tableName = tableBound.TableName;
				if (tableName != null) {
					return tableNames.Contains(tableName.Name.Value);
				}
			}
			return false;
		}

		public static bool IsTableUniqueConstraintOfTables(this IInstallStatement that, ICollection<string> tableNames) {
			AlterTableAddConstraintFragment constraint = that as AlterTableAddConstraintFragment;
			if (constraint != null) {
				return (tableNames.Contains(constraint.Owner.ObjectName)) && (constraint.Constraint is TableUniqueConstraintBase);
			}
			return false;
		}
	}
}
