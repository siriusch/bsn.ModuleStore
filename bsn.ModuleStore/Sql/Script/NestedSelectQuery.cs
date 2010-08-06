using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class NestedSelectQuery: Tuple {
		private readonly SelectQuery value;

		[Rule("<Tuple> ::= '(' <SelectQuery> ')'", ConstructorParameterMapping = new[] {1})]
		[Rule("<ExpressionParens> ::= '(' <SelectQuery> ')'", ConstructorParameterMapping = new[] {1})]
		public NestedSelectQuery(SelectQuery value): base() {
			if (value == null) {
				throw new ArgumentNullException("value");
			}
			this.value = value;
		}
	}
}