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
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	internal class FulltextMultipleTableFunction: FulltextTableFunction {
		private readonly List<ColumnName> columns;

		[Rule("<TextTableFunction> ::= CONTAINSTABLE ~'(' <TableNameQualified> ~',' ~'(' <ColumnNameList> ~')' ~',' <Expression> <OptionalContainsTop> ~')'")]
		[Rule("<TextTableFunction> ::= FREETEXTTABLE ~'(' <TableNameQualified> ~',' ~'(' <ColumnNameList> ~')' ~',' <Expression> <OptionalContainsTop> ~')'")]
		public FulltextMultipleTableFunction(ReservedKeyword keyword, Qualified<SchemaName, TableName> tableName, Sequence<ColumnName> columns, Expression query, Optional<IntegerLiteral> top): this(keyword, tableName, columns, query, null, top) {}

		[Rule("<TextTableFunction> ::= CONTAINSTABLE ~'(' <TableNameQualified> ~',' ~'(' <ColumnNameList> ~')' ~',' <Expression> ~',' ~LANGUAGE <Literal> <OptionalContainsTop> ~')'")]
		[Rule("<TextTableFunction> ::= FREETEXTTABLE ~'(' <TableNameQualified> ~',' ~'(' <ColumnNameList> ~')' ~',' <Expression> ~',' ~LANGUAGE <Literal> <OptionalContainsTop> ~')'")]
		public FulltextMultipleTableFunction(ReservedKeyword keyword, Qualified<SchemaName, TableName> tableName, Sequence<ColumnName> columns, Expression query, Literal language, Optional<IntegerLiteral> top): base(keyword, tableName, query, language, top) {
			Debug.Assert(columns != null);
			this.columns = columns.ToList();
		}

		public IEnumerable<ColumnName> Columns {
			get {
				return columns;
			}
		}

		protected override void WriteColumnInternal(SqlWriter writer) {
			writer.WriteScriptSequence(columns, WhitespacePadding.None, ", ");
		}
	}
}
