using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class UnionAllClause: UnionClause {
		[Rule("<UnionClause> ::= UNION ALL <SelectQuery>", ConstructorParameterMapping = new[] {2})]
		public UnionAllClause(SelectQuery selectQuery): base(selectQuery) {}

		public override bool All {
			get {
				return true;
			}
		}
	}
}