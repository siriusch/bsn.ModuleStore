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
	internal class FulltextSingleFunction: FulltextFunction {
		private readonly Qualified<SqlName, ColumnName> column;

		[Rule("<PredicateFunction> ::= CONTAINS ~'(' <ColumnWildNameQualified> ~',' <Expression> ~')'")]
		[Rule("<PredicateFunction> ::= FREETEXT ~'(' <ColumnWildNameQualified> ~',' <Expression> ~')'")]
		public FulltextSingleFunction(ReservedKeyword keyword, Qualified<SqlName, ColumnName> column, Expression query): this(keyword, column, query, null) {}

		[Rule("<PredicateFunction> ::= CONTAINS ~'(' <ColumnWildNameQualified> ~',' <Expression> ~',' ~LANGUAGE <Literal> ~')'")]
		[Rule("<PredicateFunction> ::= FREETEXT ~'(' <ColumnWildNameQualified> ~',' <Expression> ~',' ~LANGUAGE <Literal> ~')'")]
		public FulltextSingleFunction(ReservedKeyword keyword, Qualified<SqlName, ColumnName> column, Expression query, Literal language): base(keyword, query, language) {
			Debug.Assert(column != null);
			this.column = column;
		}

		public Qualified<SqlName, ColumnName> Column {
			get {
				return column;
			}
		}

		protected override void WriteColumnInternal(SqlWriter writer) {
			writer.WriteScript(column, WhitespacePadding.None);
		}
	}
}
