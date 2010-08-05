using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ExpressionTuple: Tuple {
		[Rule("<Tuple> ::= '(' <ExpressionList> ')'",  ConstructorParameterMapping = new[] {1})]
		public ExpressionTuple(Sequence<Expression> value): base(value) {}
	}
}