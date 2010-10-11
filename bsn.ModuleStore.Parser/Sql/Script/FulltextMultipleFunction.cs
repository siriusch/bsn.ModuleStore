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
	internal class FulltextMultipleFunction: FulltextFunction {
		private readonly List<Qualified<SqlName, ColumnName>> columns;

		[Rule("<PredicateFunction> ::= CONTAINS ~'(' ~'(' <ColumnNameQualifiedList> ~')' ~',' <Expression> ~')'")]
		[Rule("<PredicateFunction> ::= FREETEXT ~'(' ~'(' <ColumnNameQualifiedList> ~')' ~',' <Expression> ~')'")]
		public FulltextMultipleFunction(ReservedKeyword keyword, Sequence<Qualified<SqlName, ColumnName>> columns, Expression query): this(keyword, columns, query, null) {}

		[Rule("<PredicateFunction> ::= CONTAINS ~'(' ~'(' <ColumnNameQualifiedList> ~')' ~',' <Expression> ~',' ~LANGUAGE <Literal> ~')'")]
		[Rule("<PredicateFunction> ::= FREETEXT ~'(' ~'(' <ColumnNameQualifiedList> ~')' ~',' <Expression> ~',' ~LANGUAGE <Literal> ~')'")]
		public FulltextMultipleFunction(ReservedKeyword keyword, Sequence<Qualified<SqlName, ColumnName>> columns, Expression query, Literal language): base(keyword, query, language) {
			Debug.Assert(columns != null);
			this.columns = columns.ToList();
		}

		public IEnumerable<Qualified<SqlName, ColumnName>> Columns {
			get {
				return columns;
			}
		}

		protected override void WriteColumnInternal(SqlWriter writer) {
			writer.WriteScriptSequence(columns, WhitespacePadding.None, ", ");
		}
	}
}
