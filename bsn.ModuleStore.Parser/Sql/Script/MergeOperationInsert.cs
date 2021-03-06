﻿// bsn ModuleStore database versioning
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

using System.Collections.Generic;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class MergeOperationInsert: MergeOperation {
		private readonly List<ColumnName> columnNames;
		private readonly List<Expression> expressions;

		[Rule("<MergeNotMatched> ::= ~INSERT <ColumnNameGroup> ~DEFAULT ~VALUES")]
		public MergeOperationInsert(Optional<Sequence<ColumnName>> columnNames): this(columnNames, null) {}

		[Rule("<MergeNotMatched> ::= ~INSERT <ColumnNameGroup> ~VALUES ~'(' <ExpressionList> ~')'")]
		public MergeOperationInsert(Optional<Sequence<ColumnName>> columnNames, Sequence<Expression> expressions) {
			this.columnNames = columnNames.ToList();
			this.expressions = expressions.ToList();
		}

		public IEnumerable<ColumnName> ColumnNames => columnNames;

		public IEnumerable<Expression> Expressions => expressions;

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteKeyword("INSERT ");
			if (columnNames.Count > 0) {
				writer.Write('(');
				writer.WriteScriptSequence(columnNames, WhitespacePadding.None, w => w.Write(", "));
				writer.Write(") ");
			}
			if (expressions.Count > 0) {
				writer.WriteKeyword("VALUES ");
				writer.Write('(');
				writer.WriteScriptSequence(expressions, WhitespacePadding.None, w => w.Write(", "));
				writer.Write(')');
			} else {
				writer.WriteKeyword("DEFAULT VALUES");
			}
		}
	}
}
