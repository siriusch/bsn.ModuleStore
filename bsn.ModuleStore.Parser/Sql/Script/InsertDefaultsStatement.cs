using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class InsertDefaultsStatement: InsertStatement {
		[Rule("<InsertStatement> ::= <CTEGroup> ~INSERT <OptionalTop> ~<OptionalInto> <DestinationRowset> ~DEFAULT ~VALUES <QueryHint>")]
		public InsertDefaultsStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, QueryHint queryHint): base(ctes, topExpression, destinationRowset, queryHint) {}

		protected override void WriteToInternal(SqlWriter writer) {
			writer.Write(" DEFAULT VALUES");
		}
	}
}