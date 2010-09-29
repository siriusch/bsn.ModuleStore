using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class UnionAllClause: UnionClause {
		[Rule("<RowsetCombineClause> ::= ~UNION ~ALL <SelectQuery>")]
		public UnionAllClause(SelectQuery selectQuery): base(selectQuery) {}

		public override bool All {
			get {
				return true;
			}
		}

		protected override string CombineSpecifier {
			get {
				return "UNION ALL";
			}
		}
	}
}