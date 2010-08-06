using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class UpdateStatement: SqlStatement {
		private readonly Sequence<CommonTableExpression> ctes;
		private readonly DestinationRowset destinationRowset;
		private readonly FromClause fromClause;
		private readonly OutputClause outputClause;
		private readonly TopExpression topExpression;
		private readonly Sequence<UpdateItem> updateItems;
		private readonly Predicate whereClause;

		[Rule("<UpdateStatement> ::= <CTEGroup> UPDATE <Top> <DestinationRowset> SET <UpdateItemList> <OutputClause> <OptionalFromClause> <WhereClause>", ConstructorParameterMapping = new[] {0, 2, 3, 5, 6, 7, 8})]
		public UpdateStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, Sequence<UpdateItem> updateItems, OutputClause outputClause, Optional<FromClause> fromClause, Optional<Predicate> whereClause) {
			this.ctes = ctes;
			this.topExpression = topExpression;
			this.destinationRowset = destinationRowset;
			this.updateItems = updateItems;
			this.outputClause = outputClause;
			this.fromClause = fromClause;
			this.whereClause = whereClause;
		}
	}
}