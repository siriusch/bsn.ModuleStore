using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class Parens<T>: Expression where T: SqlToken {
		private readonly T value;

		[Rule("<PredicateParens> ::= '(' <Predicate> ')'", typeof(Predicate), ConstructorParameterMapping = new[] {1})]
		[Rule("<ExpressionParens> ::= '(' <Expression> ')'", typeof(Expression), ConstructorParameterMapping = new[] {1})]
		public Parens(T value) {
			this.value = value;
		}
	}
}