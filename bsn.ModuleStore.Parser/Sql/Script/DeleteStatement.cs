using System;
using System.Collections.Generic;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[CommonTableExpressionScope]
	public sealed class DeleteStatement: Statement {
		private readonly List<CommonTableExpression> ctes;
		private readonly DestinationRowset destinationRowset;
		private readonly FromClause fromClause;
		private readonly OutputClause outputClause;
		private readonly QueryHint queryHint;
		private readonly TopExpression topExpression;
		private readonly Predicate whereClause;

		[Rule("<DeleteStatement> ::= <CTEGroup> ~DELETE <OptionalTop> ~<OptionalFrom> <DestinationRowset> <OutputClause> <OptionalFromClause> <WhereClause> <QueryHint>")]
		public DeleteStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, OutputClause outputClause, Optional<FromClause> fromClause, Optional<Predicate> whereClause, QueryHint queryHint) {
			this.ctes = ctes.ToList();
			this.topExpression = topExpression;
			this.destinationRowset = destinationRowset;
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

		public TopExpression TopExpression {
			get {
				return topExpression;
			}
		}

		public Predicate WhereClause {
			get {
				return whereClause;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteCommonTableExpressions(ctes);
			writer.Write("DELETE");
			writer.WriteScript(topExpression, WhitespacePadding.SpaceBefore);
			writer.Write(" FROM ");
			writer.WriteScript(destinationRowset, WhitespacePadding.None);
			writer.WriteScript(outputClause, WhitespacePadding.SpaceBefore);
			writer.WriteScript(fromClause, WhitespacePadding.SpaceBefore);
			writer.WriteScript(whereClause, WhitespacePadding.SpaceBefore, "WHERE ", null);
			writer.WriteScript(queryHint, WhitespacePadding.SpaceBefore);
		}
	}
}