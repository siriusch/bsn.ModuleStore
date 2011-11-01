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
			writer.WriteScript(columnName, WhitespacePadding.SpaceAfter);
			writer.WriteScript(columnDefinition, WhitespacePadding.None);
		}
	}
}
