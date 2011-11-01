// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

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
