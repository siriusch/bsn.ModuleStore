using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[CommonTableExpressionScope]
	public class UpdateStatement: Statement {
		private readonly List<CommonTableExpression> ctes;
		private readonly DestinationRowset destinationRowset;
		private readonly FromClause fromClause;
		private readonly OutputClause outputClause;
		private readonly TopExpression topExpression;
		private readonly List<UpdateItem> updateItems;
		private readonly Predicate whereClause;
		private readonly QueryHint queryHint;

		[Rule("<UpdateStatement> ::= <CTEGroup> ~UPDATE <OptionalTop> <DestinationRowset> ~SET <UpdateItemList> <OutputClause> <OptionalFromClause> <WhereClause> <QueryHint>")]
		public UpdateStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, Sequence<UpdateItem> updateItems, OutputClause outputClause, Optional<FromClause> fromClause, Optional<Predicate> whereClause, QueryHint queryHint) {
			Debug.Assert(destinationRowset != null);
			this.ctes = ctes.ToList();
			this.topExpression = topExpression;
			this.destinationRowset = destinationRowset;
			this.updateItems = updateItems.ToList();
			this.outputClause = outputClause;
			this.fromClause = fromClause;
			this.whereClause = whereClause;
			this.queryHint = queryHint;
		}

		public IEnumerable<CommonTableExpression> Ctes {
			get {
				return ctes;
			}
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

		public QueryHint QueryHint {
			get {
				return queryHint;
			}
		}

		public OutputClause OutputClause {
			get {
				return outputClause;
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
			writer.WriteCommonTableExpressions(ctes);
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