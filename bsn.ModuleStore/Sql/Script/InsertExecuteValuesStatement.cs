using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class InsertExecuteValuesStatement: InsertValuesStatement {
		[Rule("<InsertStatement> ::= <CTEGroup> INSERT <Top> <OptionalInto> <DestinationRowset> <ColumnNameGroup> <OutputClause> <ExecuteStatement>", ConstructorParameterMapping = new[] {0, 2, 4, 5, 6, 7})]
		public InsertExecuteValuesStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, Optional<Sequence<ColumnName>> columnNames, OutputClause output, ExecuteStatement executeStatement)
				: base(ctes, topExpression, destinationRowset, columnNames, output) {}
	}
}