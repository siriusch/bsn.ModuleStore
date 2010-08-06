using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ExpressionCountFunction: ExpressionFunction {
		[Rule("<ExpressionFunction> ::= COUNT_ <Restriction> <ColumnWildNameQualified> ')'", ConstructorParameterMapping = new[] {1, 2})]
		public ExpressionCountFunction(DuplicateRestriction restriction, Qualified<ColumnName> columnName) {}
	}
}