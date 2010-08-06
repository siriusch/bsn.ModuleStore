using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CaseWhen<T>: SqlToken where T: SqlComputable {
		private readonly T condition;
		private readonly Expression value;

		[Rule("<CaseWhenExpression> ::= WHEN <Expression> THEN <Expression>", typeof(Expression), ConstructorParameterMapping = new[] {1, 3})]
		[Rule("<CaseWhenPredicate> ::= WHEN <Predicate> THEN <Expression>", typeof(Predicate), ConstructorParameterMapping = new[] {1, 3})]
		public CaseWhen(T condition, Expression value) {
			this.condition = condition;
			this.value = value;
		}
	}
}