using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ColumnCollateConstraint: ColumnConstraint {
		[Rule("<ColumnConstraint> ::= COLLATE <CollationName>", ConstructorParameterMapping = new[] {1})]
		public ColumnCollateConstraint(CollationName collation) {}
	}
}