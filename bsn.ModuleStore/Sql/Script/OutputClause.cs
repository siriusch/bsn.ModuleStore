using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class OutputClause: SqlToken {
		[Rule("<OutputClause> ::=")]
		public OutputClause(): this(null, null, null) {}

		[Rule("<OutputClause> ::= OUTPUT <ColumnItemList>", ConstructorParameterMapping = new[] {1})]
		public OutputClause(Sequence<ColumnItem> columnItems): this(columnItems, null, null) {}

		[Rule("<OutputClause> ::= OUTPUT <ColumnItemList> INTO <DestinationRowset> <ColumnNameGroup>", ConstructorParameterMapping = new[] {1, 3, 4})]
		public OutputClause(Sequence<ColumnItem> columnItems, DestinationRowset destinationName, Optional<Sequence<ColumnName>> destinationColumnNames) {}
	}
}