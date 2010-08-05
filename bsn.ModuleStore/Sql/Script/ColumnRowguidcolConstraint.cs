using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ColumnRowguidcolConstraint: ColumnConstraint {
		[Rule("<ColumnConstraint> ::= ROWGUIDCOL", AllowTruncationForConstructor = true)]
		public ColumnRowguidcolConstraint() {}
	}
}