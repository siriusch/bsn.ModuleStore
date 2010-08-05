using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class InsertSelectValuesStatement: InsertValuesStatement {
		[Rule("<InsertStatement> ::= <CTEGroup> INSERT <Top> <OptionalInto> <DestinationRowset> <ColumnNameGroup> <OutputClause> <SelectQuery>", ConstructorParameterMapping = new[] {0, 2, 4, 5, 6, 7})]
		public InsertSelectValuesStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, Optional<Sequence<ColumnName>> columnNames, OutputClause output, SelectQuery selectQuery)
				: base(ctes, topExpression, destinationRowset, columnNames, output) {}
	}
}