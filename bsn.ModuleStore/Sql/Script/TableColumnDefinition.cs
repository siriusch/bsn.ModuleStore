using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TableColumnDefinition: TableDefinition {
		internal static void AssertIsNotWildcard(ColumnName columnName) {
			if (columnName.IsWildcard) {
				throw new ArgumentException("Wilcard column names are not allowed for table column definitions", "columnName");
			}
		}

		private readonly ColumnDefinition columnDefinition;
		private readonly ColumnName columnName;

		[Rule("<TableDefinition> ::= <ColumnName> <ColumnDefinition>")]
		public TableColumnDefinition(ColumnName columnName, ColumnDefinition columnDefinition) {
			Debug.Assert(columnName != null);
			Debug.Assert(columnDefinition != null);
			AssertIsNotWildcard(columnName);
			this.columnName = columnName;
			this.columnDefinition = columnDefinition;
		}

		public ColumnDefinition ColumnDefinition {
			get {
				return columnDefinition;
			}
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(columnName);
			writer.Write(' ');
			writer.WriteScript(columnDefinition);
		}
	}
}