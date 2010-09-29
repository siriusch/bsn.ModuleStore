using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ExceptClause: RowsetCombineClause {
		[Rule("<RowsetCombineClause> ::= ~EXCEPT <SelectQuery>")]
		public ExceptClause(SelectQuery selectQuery): base(selectQuery) {}

		protected override string CombineSpecifier {
			get {
				return "EXCEPT";
			}
		}
	}
}