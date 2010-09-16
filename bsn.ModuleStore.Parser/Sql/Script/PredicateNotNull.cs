using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PredicateNotNull: PredicateNull {
		[Rule("<PredicateNull> ::= <Expression> ~IS ~NOT ~NULL")]
		public PredicateNotNull(Expression valueExpression): base(valueExpression, true) {}
	}
}