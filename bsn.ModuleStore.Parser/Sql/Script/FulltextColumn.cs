using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class FulltextColumn: SqlScriptableToken {
		private readonly ColumnName columnName;
		private readonly Literal language;
		private readonly Qualified<SchemaName, TypeName> typeColumn;

		[Rule("<FulltextColumn> ::= <ColumnName> <FulltextColumnType> <OptionalLanguage>")]
		public FulltextColumn(ColumnName columnName, Optional<Qualified<SchemaName, TypeName>> typeColumn, Optional<Literal> language) {
			Debug.Assert(columnName != null);
			this.columnName = columnName;
			this.typeColumn = typeColumn;
			this.language = language;
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}

		public Literal Language {
			get {
				return language;
			}
		}

		public Qualified<SchemaName, TypeName> TypeColumn {
			get {
				return typeColumn;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(columnName, WhitespacePadding.None);
			writer.WriteScript(typeColumn, WhitespacePadding.SpaceBefore, "TYPE COLUMN ", null);
			writer.WriteScript(language, WhitespacePadding.SpaceBefore, "LANGUAGE ", null);
		}
	}
}