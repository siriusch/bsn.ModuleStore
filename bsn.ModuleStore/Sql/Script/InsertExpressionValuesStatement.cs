using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class InsertExpressionValuesStatement: InsertValuesStatement {
		[Rule("<InsertStatement> ::= <CTEGroup> INSERT <Top> <OptionalInto> <DestinationRowset> <ColumnNameGroup> <OutputClause> VALUES '(' <ExpressionList> ')'", ConstructorParameterMapping = new[] {0, 2, 4, 5, 6, 9})]
		public InsertExpressionValuesStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, Optional<Sequence<ColumnName>> columnNames, OutputClause output, Sequence<Expression> expressions)
				: base(ctes, topExpression, destinationRowset, columnNames, output) {}
	}
}