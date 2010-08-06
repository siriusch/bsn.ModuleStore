using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ExpressionTuple: Tuple {
		private readonly Sequence<Expression> value;

		[Rule("<Tuple> ::= '(' <ExpressionList> ')'", ConstructorParameterMapping = new[] {1})]
		public ExpressionTuple(Sequence<Expression> value): base() {
			if (value == null) {
				throw new ArgumentNullException("value");
			}
			this.value = value;
		}
	}
}