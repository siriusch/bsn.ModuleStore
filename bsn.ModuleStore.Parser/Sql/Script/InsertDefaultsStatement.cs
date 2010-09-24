using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class InsertDefaultsStatement: InsertStatement {
		[Rule("<InsertStatement> ::= <CTEGroup> ~INSERT <OptionalTop> ~<OptionalInto> <DestinationRowset> ~DEFAULT ~VALUES <QueryHint>")]
		public InsertDefaultsStatement(QueryOptions queryOptions, TopExpression topExpression, DestinationRowset destinationRowset, QueryHint queryHint): base(queryOptions, topExpression, destinationRowset, queryHint) {}

		protected override void WriteToInternal(SqlWriter writer) {
			writer.Write(" DEFAULT VALUES");
		}
	}
}