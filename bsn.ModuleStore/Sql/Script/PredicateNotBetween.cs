using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PredicateNotBetween: PredicateBetween {
		[Rule("<PredicateBetween> ::= <Expression> NOT BETWEEN <Expression> AND <Expression>", ConstructorParameterMapping=new[] { 0, 3, 5 })]
		public PredicateNotBetween(Expression valueExpression, Expression min, Expression max) : base(valueExpression, true, min, max) {
		}
	}
}