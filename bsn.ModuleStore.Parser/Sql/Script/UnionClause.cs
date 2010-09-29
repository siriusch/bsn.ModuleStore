using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class UnionClause: RowsetCombineClause {
		[Rule("<RowsetCombineClause> ::= ~UNION <SelectQuery>")]
		public UnionClause(SelectQuery selectQuery): base(selectQuery) {}

		public virtual bool All {
			get {
				return false;
			}
		}

		protected override string CombineSpecifier {
			get {
				return "UNION";
			}
		}
	}
}