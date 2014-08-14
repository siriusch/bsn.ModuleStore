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
	[CommonTableExpressionScope]
	public sealed class MergeStatement: Statement {
		private readonly RowsetAlias destinationAlias;
		private readonly DestinationRowset destinationRowset;
		private readonly OutputClause outputClause;
		private readonly Predicate predicate;
		private readonly QueryHint queryHint;
		private readonly QueryOptions queryOptions;
		private readonly SourceRowset sourceRowset;
		private readonly TopExpression topExpression;
		private readonly List<MergeWhenMatched> whenMatchedCollection;

		[Rule("<MergeStatement> ::= <QueryOptions> ~MERGE <OptionalTop> ~<OptionalInto> <DestinationRowset> <RowsetAlias> ~USING <SourceRowset> ~ON <Predicate> <MergeWhenMatchedList> <OutputClause> <QueryHint>")]
		public MergeStatement(QueryOptions queryOptions, TopExpression topExpression, DestinationRowset destinationRowset, RowsetAlias destinationAlias, SourceRowset sourceRowset, Predicate predicate, Sequence<MergeWhenMatched> mergeWhenMatcheds, OutputClause outputClause, QueryHint queryHint) {
			this.queryOptions = queryOptions;
			this.topExpression = topExpression;
			this.destinationRowset = destinationRowset;
			this.destinationAlias = destinationAlias;
			this.sourceRowset = sourceRowset;
			this.predicate = predicate;
			whenMatchedCollection = mergeWhenMatcheds.ToList();
			this.outputClause = outputClause;
			this.queryHint = queryHint;
		}

		public RowsetAlias DestinationAlias {
			get {
				return destinationAlias;
			}
		}

		public DestinationRowset DestinationRowset {
			get {
				return destinationRowset;
			}
		}

		public OutputClause OutputClause {
			get {
				return outputClause;
			}
		}

		public Predicate Predicate {
			get {
				return predicate;
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

		public SourceRowset SourceRowset {
			get {
				return sourceRowset;
			}
		}

		public TopExpression TopExpression {
			get {
				return topExpression;
			}
		}

		public IEnumerable<MergeWhenMatched> WhenMatchedCollection {
			get {
				return whenMatchedCollection;
			}
		}

		public override sealed void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteScript(queryOptions, WhitespacePadding.NewlineAfter);
			writer.WriteKeyword("MERGE ");
			using (writer.Indent()) {
				writer.WriteScript(topExpression, WhitespacePadding.SpaceAfter);
				writer.WriteLine();
				writer.WriteKeyword("INTO ");
				using (writer.Indent()) {
					writer.WriteScript(destinationRowset, WhitespacePadding.None);
					writer.WriteScript(destinationAlias, WhitespacePadding.SpaceBefore);
				}
				writer.WriteLine();
				writer.WriteKeyword("USING ");
				using (writer.Indent()) {
					writer.WriteScript(sourceRowset, WhitespacePadding.None);
					writer.WriteKeyword(" ON ");
					writer.WriteScript(predicate, WhitespacePadding.None);
				}
				writer.WriteScriptSequence(whenMatchedCollection, WhitespacePadding.NewlineBefore, null);
				writer.WriteScript(outputClause, WhitespacePadding.NewlineBefore);
				writer.WriteScript(queryHint, WhitespacePadding.NewlineBefore);
			}
		}
	}
}
