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

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class InsertExpressionValuesStatement: InsertValuesStatement {
		private readonly List<Expression> expressions;

		[Rule("<InsertStatement> ::= <QueryOptions> ~INSERT <OptionalTop> ~<OptionalInto> <DestinationRowset> <ColumnNameGroup> <OutputClause> ~VALUES ~'(' <ExpressionList> ~')' <QueryHint>")]
		public InsertExpressionValuesStatement(QueryOptions queryOptions, TopExpression topExpression, DestinationRowset destinationRowset, Optional<Sequence<ColumnName>> columnNames, OutputClause output, Sequence<Expression> expressions, QueryHint queryHint)
				: base(queryOptions, topExpression, destinationRowset, columnNames, output, queryHint) {
			this.expressions = expressions.ToList();
		}

		public IEnumerable<Expression> Expressions {
			get {
				return expressions;
			}
		}

		protected override void WriteToInternal(SqlWriter writer) {
			base.WriteToInternal(writer);
			writer.WriteLine();
			writer.Write("VALUES (");
			writer.WriteScriptSequence(expressions, WhitespacePadding.None, ", ");
			writer.Write(')');
		}
	}
}
