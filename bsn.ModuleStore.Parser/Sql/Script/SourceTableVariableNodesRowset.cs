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
//  
using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceTableVariableNodesRowset: SourceNodesRowset {
		private readonly ColumnName columnName;
		private readonly VariableName tableName;

		[Rule("<SourceRowset> ::= <VariableName> ~'.' <ColumnName> ~'.' <NamedFunction> <RowsetAlias>")]
		public SourceTableVariableNodesRowset(VariableName tableName, ColumnName columnName, NamedFunction functionCall, RowsetAlias rowsetAlias): base(functionCall, rowsetAlias) {
			Debug.Assert(tableName != null);
			Debug.Assert(columnName != null);
			this.tableName = tableName;
			this.columnName = columnName;
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}

		public VariableName TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(tableName, WhitespacePadding.None);
			writer.Write('.');
			writer.WriteScript(columnName, WhitespacePadding.None);
			base.WriteTo(writer);
		}
	}
}
