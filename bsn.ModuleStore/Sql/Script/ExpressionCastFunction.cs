using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ExpressionCastFunction: ExpressionFunction {
		[Rule("<ExpressionFunction> ::= CAST_ <Expression> AS <TypeName> ')'", ConstructorParameterMapping = new[] {1, 3})]
		public ExpressionCastFunction(Expression expression, TypeName typeName) {}
	}
}