using System;
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
			if (columnName == null) {
				throw new ArgumentNullException("columnName");
			}
			AssertIsNotWildcard(columnName);
			if (columnDefinition == null) {
				throw new ArgumentNullException("columnDefinition");
			}
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

		public override void WriteTo(TextWriter writer) {
			writer.WriteScript(columnName);
			writer.Write(' ');
			writer.WriteScript(columnDefinition);
		}
	}
}