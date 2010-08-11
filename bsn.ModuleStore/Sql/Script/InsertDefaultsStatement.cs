using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class InsertDefaultsStatement: InsertStatement {
		[Rule("<InsertStatement> ::= <CTEGroup> INSERT <OptionalTop> <OptionalInto> <DestinationRowset> DEFAULT VALUES <QueryHint>", ConstructorParameterMapping = new[] {0, 2, 4, 7})]
		public InsertDefaultsStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, QueryHint queryHint): base(ctes, topExpression, destinationRowset, queryHint) {}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write(" DEFAULT VALUES");
			writer.WriteScript(QueryHint, WhitespacePadding.SpaceBefore);
		}
	}
}