using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class Tuple: Parens<Sequence<Expression>> {
		[Rule("<Tuple> ::= '(' <SelectQuery> ')'", ConstructorParameterMapping = new[] {1})]
		public Tuple(Sequence<Expression> value): base(value) {}
	}
}