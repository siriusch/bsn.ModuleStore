using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DeleteStatement: Statement {
		private readonly List<CommonTableExpression> ctes;
		private readonly DestinationRowset destinationRowset;
		private readonly FromClause fromClause;
		private readonly OutputClause outputClause;
		private readonly TopExpression topExpression;
		private readonly Predicate whereClause;
		private readonly QueryHint queryHint;

		[Rule("<DeleteStatement> ::= <CTEGroup> DELETE <OptionalTop> <OptionalFrom> <DestinationRowset> <OutputClause> <OptionalFromClause> <WhereClause> <QueryHint>", ConstructorParameterMapping=new[] { 0, 2, 4, 5, 6, 7, 8 })]
		public DeleteStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, OutputClause outputClause, Optional<FromClause> fromClause, Optional<Predicate> whereClause, QueryHint queryHint) {
			this.ctes = ctes.ToList();
			this.topExpression = topExpression;
			this.destinationRowset = destinationRowset;
			this.outputClause = outputClause;
			this.fromClause = fromClause;
			this.whereClause = whereClause;
			this.queryHint = queryHint;
		}

		public List<CommonTableExpression> Ctes {
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

		public QueryHint QueryHint {
			get {
				return queryHint;
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