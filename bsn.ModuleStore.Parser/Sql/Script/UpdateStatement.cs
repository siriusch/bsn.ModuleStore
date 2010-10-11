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
	[CommonTableExpressionScope]
	public class UpdateStatement: Statement {
		private readonly DestinationRowset destinationRowset;
		private readonly FromClause fromClause;
		private readonly OutputClause outputClause;
		private readonly QueryHint queryHint;
		private readonly QueryOptions queryOptions;
		private readonly TopExpression topExpression;
		private readonly List<UpdateItem> updateItems;
		private readonly Predicate whereClause;

		[Rule("<UpdateStatement> ::= <QueryOptions> ~UPDATE <OptionalTop> <DestinationRowset> ~SET <UpdateItemList> <OutputClause> <OptionalFromClause> <WhereClause> <QueryHint>")]
		public UpdateStatement(QueryOptions queryOptions, TopExpression topExpression, DestinationRowset destinationRowset, Sequence<UpdateItem> updateItems, OutputClause outputClause, Optional<FromClause> fromClause, Optional<Predicate> whereClause, QueryHint queryHint) {
			Debug.Assert(destinationRowset != null);
			this.queryOptions = queryOptions;
			this.topExpression = topExpression;
			this.destinationRowset = destinationRowset;
			this.updateItems = updateItems.ToList();
			this.outputClause = outputClause;
			this.fromClause = fromClause;
			this.whereClause = whereClause;
			this.queryHint = queryHint;
		}

		public DestinationRowset DestinationRowset {
			get {
				return destinationRowset;
			}
		}

		public FromClause FromClause {
			get {
				return fromClause;
			}
		}

		public OutputClause OutputClause {
			get {
				return outputClause;
			}
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

		public TopExpression TopExpression {
			get {
				return topExpression;
			}
		}

		public IEnumerable<UpdateItem> UpdateItems {
			get {
				return updateItems;
			}
		}

		public Predicate WhereClause {
			get {
				return whereClause;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteScript(queryOptions, WhitespacePadding.NewlineAfter);
			writer.Write("UPDATE ");
			writer.IncreaseIndent();
			writer.WriteScript(topExpression, WhitespacePadding.SpaceAfter);
			writer.WriteScript(destinationRowset, WhitespacePadding.None);
			writer.WriteLine();
			writer.Write("SET ");
			writer.WriteScriptSequence(updateItems, WhitespacePadding.None, ", ");
			writer.WriteScript(outputClause, WhitespacePadding.NewlineBefore);
			writer.WriteScript(fromClause, WhitespacePadding.NewlineBefore);
			writer.WriteScript(whereClause, WhitespacePadding.NewlineBefore, "WHERE ", null);
			writer.WriteScript(queryHint, WhitespacePadding.SpaceBefore);
			writer.DecreaseIndent();
		}
	}
}
