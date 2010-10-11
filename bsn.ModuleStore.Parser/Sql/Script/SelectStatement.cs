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
	[CommonTableExpressionScope]
	public sealed class SelectStatement: Statement {
		private readonly QueryHint queryHint;
		private readonly QueryOptions queryOptions;
		private readonly SelectQuery selectQuery;

		[Rule("<SelectStatement> ::= <QueryOptions> <SelectQuery> <QueryHint>")]
		public SelectStatement(QueryOptions queryOptions, SelectQuery selectQuery, QueryHint queryHint) {
			Debug.Assert(selectQuery != null);
			this.queryOptions = queryOptions;
			this.selectQuery = selectQuery;
			this.queryHint = queryHint;
		}

		public QueryHint QueryHint {
			get {
				return queryHint;
			}
		}

		public QueryOptions QueryOptions {
			get {
				return queryOptions;
			}
		}

		public SelectQuery SelectQuery {
			get {
				return selectQuery;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteScript(queryOptions, WhitespacePadding.NewlineAfter);
			writer.IncreaseIndent();
			writer.WriteScript(selectQuery, WhitespacePadding.None);
			writer.WriteScript(queryHint, WhitespacePadding.SpaceBefore);
			writer.DecreaseIndent();
		}
	}
}
