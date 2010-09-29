using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class IntersectClause: RowsetCombineClause {
		[Rule("<RowsetCombineClause> ::= ~INTERSECT <SelectQuery>")]
		public IntersectClause(SelectQuery selectQuery): base(selectQuery) {}

		protected override string CombineSpecifier {
			get {
				return "INTERSECT";
			}
		}
	}
}