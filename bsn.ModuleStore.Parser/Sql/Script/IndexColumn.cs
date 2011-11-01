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
using System.Data.SqlClient;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class IndexColumn: SqlScriptableToken {
		private readonly ColumnName columnName;
		private readonly SortOrder order;

		[Rule("<IndexColumn> ::= <ColumnName> <OrderType>")]
		public IndexColumn(ColumnName columnName, OrderTypeToken order) {
			this.columnName = columnName;
			this.order = order.Order;
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}

		public SortOrder Order {
			get {
				return order;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(columnName, WhitespacePadding.None);
			writer.WriteEnum(order, WhitespacePadding.SpaceBefore);
		}
	}
}
