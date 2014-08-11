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

namespace bsn.ModuleStore.Sql.Script {
	public abstract class InsertValuesStatement: InsertStatement {
		private readonly List<ColumnName> columnNames;
		private readonly OutputClause output;

		protected InsertValuesStatement(QueryOptions queryOptions, TopExpression topExpression, DestinationRowset destinationRowset, Optional<Sequence<ColumnName>> columnNames, OutputClause output, QueryHint queryHint): base(queryOptions, topExpression, destinationRowset, queryHint) {
			Debug.Assert(output != null);
			this.columnNames = columnNames.ToList();
			this.output = output;
		}

		public IEnumerable<ColumnName> ColumnNames {
			get {
				return columnNames;
			}
		}

		public OutputClause Output {
			get {
				return output;
			}
		}

		protected override void WriteToInternal(SqlWriter writer) {
			if (columnNames.Count > 0) {
				writer.Write(" (");
				writer.WriteScriptSequence(columnNames, WhitespacePadding.None, w => w.Write(", "));
				writer.Write(")");
			}
			writer.WriteScript(output, WhitespacePadding.NewlineBefore);
		}
	}
}
