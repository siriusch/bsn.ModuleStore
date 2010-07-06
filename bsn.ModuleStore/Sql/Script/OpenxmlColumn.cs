using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class OpenxmlColumn: SqlToken {
		private readonly ColumnName columnName;
		private readonly TypeName columnType;
		private readonly StringLiteral columnPattern;

		[Rule("<OpenxmlColumn> ::= <ColumnName> <TypeName>")]
		public OpenxmlColumn(ColumnName columnName, TypeName columnType): this(columnName, columnType, null) {
		}

		[Rule("<OpenxmlColumn> ::= <ColumnName> <TypeName> <StringLiteral>")]
		public OpenxmlColumn(ColumnName columnName, TypeName columnType, StringLiteral columnPattern) {
			if (columnName == null) {
				throw new ArgumentNullException("columnName");
			}
			if (columnType == null) {
				throw new ArgumentNullException("columnType");
			}
			this.columnName = columnName;
			this.columnType = columnType;
			this.columnPattern = columnPattern;
		}
	}
}