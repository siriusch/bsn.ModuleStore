using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class InsertDefaultsStatement: InsertStatement {
		[Rule("<InsertStatement> ::= <CTEGroup> INSERT <Top> <OptionalInto> <DestinationRowset> DEFAULT VALUES", ConstructorParameterMapping = new[] {0, 2, 4})]
		public InsertDefaultsStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset): base(ctes, topExpression, destinationRowset) {}
	}
}