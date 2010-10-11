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
	internal class FulltextSingleTableFunction: FulltextTableFunction {
		private readonly ColumnName column;

		[Rule("<TextTableFunction> ::= CONTAINSTABLE ~'(' <TableNameQualified> ~',' <ColumnName> ~',' <Expression> <OptionalContainsTop> ~')'")]
		[Rule("<TextTableFunction> ::= CONTAINSTABLE ~'(' <TableNameQualified> ~',' <ColumnWild> ~',' <Expression> <OptionalContainsTop> ~')'")]
		[Rule("<TextTableFunction> ::= FREETEXTTABLE ~'(' <TableNameQualified> ~',' <ColumnName> ~',' <Expression> <OptionalContainsTop> ~')'")]
		[Rule("<TextTableFunction> ::= FREETEXTTABLE ~'(' <TableNameQualified> ~',' <ColumnWild> ~',' <Expression> <OptionalContainsTop> ~')'")]
		public FulltextSingleTableFunction(ReservedKeyword keyword, Qualified<SchemaName, TableName> tableName, ColumnName column, Expression query, Optional<IntegerLiteral> top): this(keyword, tableName, column, query, null, top) {}

		[Rule("<TextTableFunction> ::= CONTAINSTABLE ~'(' <TableNameQualified> ~',' <ColumnName> ~',' <Expression> ~',' ~LANGUAGE <Literal> <OptionalContainsTop> ~')'")]
		[Rule("<TextTableFunction> ::= CONTAINSTABLE ~'(' <TableNameQualified> ~',' <ColumnWild> ~',' <Expression> ~',' ~LANGUAGE <Literal> <OptionalContainsTop> ~')'")]
		[Rule("<TextTableFunction> ::= FREETEXTTABLE ~'(' <TableNameQualified> ~',' <ColumnName> ~',' <Expression> ~',' ~LANGUAGE <Literal> <OptionalContainsTop> ~')'")]
		[Rule("<TextTableFunction> ::= FREETEXTTABLE ~'(' <TableNameQualified> ~',' <ColumnWild> ~',' <Expression> ~',' ~LANGUAGE <Literal> <OptionalContainsTop> ~')'")]
		public FulltextSingleTableFunction(ReservedKeyword keyword, Qualified<SchemaName, TableName> tableName, ColumnName column, Expression query, Literal language, Optional<IntegerLiteral> top): base(keyword, tableName, query, language, top) {
			Debug.Assert(column != null);
			this.column = column;
		}

		public ColumnName Column {
			get {
				return column;
			}
		}

		protected override void WriteColumnInternal(SqlWriter writer) {
			writer.WriteScript(column, WhitespacePadding.None);
		}
	}
}
