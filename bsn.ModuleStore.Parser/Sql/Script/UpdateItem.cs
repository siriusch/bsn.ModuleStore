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

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class UpdateItem: SqlScriptableToken {
		private readonly Qualified<SqlName, ColumnName> columnName;
		private readonly Expression expression;
		private readonly VariableName variableName;

		[Rule("<UpdateItem> ::= <ColumnNameQualified> ~'=' ~DEFAULT")]
		public UpdateItem(Qualified<SqlName, ColumnName> columnName): this(null, columnName, null) {}

		[Rule("<UpdateItem> ::= <VariableName> ~'=' <Expression>")]
		public UpdateItem(VariableName variableName, Expression expression): this(variableName, null, expression) {}

		[Rule("<UpdateItem> ::= <ColumnNameQualified> ~'=' <Expression>")]
		public UpdateItem(Qualified<SqlName, ColumnName> columnName, Expression expression): this(null, columnName, expression) {}

		[Rule("<UpdateItem> ::= <VariableName> ~'=' <ColumnNameQualified> ~'=' <Expression>")]
		public UpdateItem(VariableName variableName, Qualified<SqlName, ColumnName> columnName, Expression expression) {
			this.columnName = columnName;
			this.variableName = variableName;
			this.expression = expression;
		}

		public Qualified<SqlName, ColumnName> ColumnName {
			get {
				return columnName;
			}
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public VariableName VariableName {
			get {
				return variableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(variableName, WhitespacePadding.None, null, "=");
			writer.WriteScript(columnName, WhitespacePadding.None, null, "=");
			if (expression == null) {
				writer.Write("DEFAULT");
			} else {
				writer.WriteScript(expression, WhitespacePadding.None);
			}
		}
	}
}
