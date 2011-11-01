// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Ars�ne von Wyss - avw@gmx.ch
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

using System.Collections.Generic;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class UpdateMethodItem: UpdateItem {
		private readonly List<NamedFunction> methods;

		[Rule("<UpdateItem> ::= <VariableName> ~'.' <NamedFunctionList>")]
		public UpdateMethodItem(VariableName variableName, Sequence<NamedFunction> methods): this(variableName, null, methods) {}

		[Rule("<UpdateItem> ::= <TableName> ~'.' <NamedFunctionList>")]
		public UpdateMethodItem(TableName tableName, Sequence<NamedFunction> methods): this(null, new Qualified<SqlName, ColumnName>(null, new ColumnName(tableName.Value)), methods) {}

		[Rule("<UpdateItem> ::= <TableName> ~'.' <ColumnName> ~'.' <NamedFunctionList>")]
		public UpdateMethodItem(TableName tableName, ColumnName columnName, Sequence<NamedFunction> methods): this(null, new Qualified<SqlName, ColumnName>(tableName, columnName), methods) {}

		private UpdateMethodItem(VariableName variableName, Qualified<SqlName, ColumnName> columnName, Sequence<NamedFunction> methods): base(columnName, variableName) {
			this.methods = methods.ToList();
		}

		public IEnumerable<NamedFunction> Methods {
			get {
				return methods;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(VariableName, WhitespacePadding.None, null, ".");
			writer.WriteScript(ColumnName, WhitespacePadding.None, null, ".");
			writer.WriteScriptSequence(methods, WhitespacePadding.None, ".");
		}
	}
}
