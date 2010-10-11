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

using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ExpressionColumnFunctionCalls: ExpressionFunctionCalls {
		private readonly Qualified<TableName, ColumnName> columnNameQualified;

		[Rule("<Value> ::= <TableName> ~'.' <ColumnName> ~'.' <NamedFunctionList>")]
		public ExpressionColumnFunctionCalls(TableName tableName, ColumnName columnName, Sequence<NamedFunction> functions): base(functions.Item, functions.Next) {
			Debug.Assert(tableName != null);
			Debug.Assert(columnName != null);
			columnNameQualified = new Qualified<TableName, ColumnName>(tableName, columnName);
			Debug.Assert(!functions.Item.FunctionName.IsQualified);
			functions.Item.FunctionName.LockOverride();
		}

		public Qualified<TableName, ColumnName> ColumnNameQualified {
			get {
				return columnNameQualified;
			}
		}

		protected override void WriteToInternal(SqlWriter writer) {
			writer.WriteScript(columnNameQualified, WhitespacePadding.None);
			writer.Write('.');
			base.WriteToInternal(writer);
		}
	}
}
