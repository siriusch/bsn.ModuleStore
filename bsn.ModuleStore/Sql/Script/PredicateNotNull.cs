using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PredicateNotNull: PredicateNull {
		[Rule("<PredicateNull> ::= <Expression> IS NOT NULL", ConstructorParameterMapping = new[] {2})]
		public PredicateNotNull(Expression valueExpression): base(valueExpression, true) {}
	}
}