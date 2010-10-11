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
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CommonTableExpression: SqlScriptableToken {
		private readonly AliasName aliasName;
		private readonly List<ColumnName> columnNames;
		private readonly SelectQuery selectQuery;

		[Rule("<CTE> ::= <AliasName> <ColumnNameGroup> ~AS ~'(' <SelectQuery> ~')'")]
		public CommonTableExpression(AliasName aliasName, Optional<Sequence<ColumnName>> columnNames, SelectQuery selectQuery) {
			Debug.Assert(aliasName != null);
			Debug.Assert(selectQuery != null);
			this.aliasName = aliasName;
			this.columnNames = columnNames.ToList();
			this.selectQuery = selectQuery;
		}

		public AliasName AliasName {
			get {
				return aliasName;
			}
		}

		public IEnumerable<ColumnName> ColumnNames {
			get {
				return columnNames;
			}
		}

		public SelectQuery SelectQuery {
			get {
				return selectQuery;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(aliasName, WhitespacePadding.None);
			if (columnNames.Count > 0) {
				writer.Write(" (");
				writer.WriteScriptSequence(columnNames, WhitespacePadding.None, ", ");
				writer.Write(')');
			}
			writer.Write(" AS (");
			writer.IncreaseIndent();
			writer.WriteScript(selectQuery, WhitespacePadding.NewlineBefore);
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write(')');
		}
	}
}
