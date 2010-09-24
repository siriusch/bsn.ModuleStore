using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

		[Rule("<UpdateStatement> ::= <CTEGroup> ~UPDATE <OptionalTop> <DestinationRowset> ~SET <UpdateItemList> <OutputClause> <OptionalFromClause> <WhereClause> <QueryHint>")]
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
			writer.WriteScript(outputClause, WhitespacePadding.SpaceBefore);
			writer.WriteScript(fromClause, WhitespacePadding.SpaceBefore);
			writer.WriteScript(whereClause, WhitespacePadding.SpaceBefore, "WHERE ", null);
			writer.WriteScript(queryHint, WhitespacePadding.SpaceBefore);
			writer.DecreaseIndent();
		}
	}
}