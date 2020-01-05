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

using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[CommonTableExpressionScope]
	public sealed class DeleteStatement: Statement {
		private readonly DestinationRowset destinationRowset;
		private readonly FromClause fromClause;
		private readonly OutputClause outputClause;
		private readonly QueryHint queryHint;
		private readonly QueryOptions queryOptions;
		private readonly TopExpression topExpression;
		private readonly Predicate whereClause;

		[Rule("<DeleteStatement> ::= <QueryOptions> ~DELETE <OptionalTop> ~<OptionalFrom> <DestinationRowset> <OutputClause> <OptionalFromClause> <WhereClause> <QueryHint>")]
		public DeleteStatement(QueryOptions queryOptions, TopExpression topExpression, DestinationRowset destinationRowset, OutputClause outputClause, Optional<FromClause> fromClause, Optional<Predicate> whereClause, QueryHint queryHint) {
			this.queryOptions = queryOptions;
			this.topExpression = topExpression;
			this.destinationRowset = destinationRowset;
			this.outputClause = outputClause;
			this.fromClause = fromClause;
			this.whereClause = whereClause;
			this.queryHint = queryHint;
		}

		public DestinationRowset DestinationRowset => destinationRowset;

		public FromClause FromClause => fromClause;

		public OutputClause OutputClause => outputClause;

		public QueryHint QueryHint => queryHint;

		public QueryOptions QueryOptions => queryOptions;

		public TopExpression TopExpression => topExpression;

		public Predicate WhereClause => whereClause;

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteScript(queryOptions, WhitespacePadding.NewlineAfter);
			writer.WriteKeyword("DELETE");
			using (writer.Indent()) {
				writer.WriteScript(topExpression, WhitespacePadding.SpaceBefore);
				writer.WriteLine();
				writer.WriteKeyword("FROM ");
				writer.WriteScript(destinationRowset, WhitespacePadding.None);
				writer.WriteScript(outputClause, WhitespacePadding.SpaceBefore);
				writer.WriteScript(fromClause, WhitespacePadding.SpaceBefore);
				writer.WriteScript(whereClause, WhitespacePadding.SpaceBefore, w => w.WriteKeyword("WHERE "), null);
				writer.WriteScript(queryHint, WhitespacePadding.SpaceBefore);
			}
		}
	}
}
