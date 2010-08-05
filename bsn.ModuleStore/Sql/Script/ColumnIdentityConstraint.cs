using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ColumnIdentityConstraint: ColumnConstraint {
		[Rule("<ColumnConstraint> ::= IDENTITY", AllowTruncationForConstructor = true)]
		public ColumnIdentityConstraint() {}

		[Rule("<ColumnConstraint> ::= IDENTITY '(' IntegerLiteral ',' IntegerLiteral ')'", ConstructorParameterMapping = new[] {2, 4})]
		public ColumnIdentityConstraint(IntegerLiteral seed, IntegerLiteral increment) {}
	}
}