using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ColumnDefaultConstraint: ColumnConstraint {
		[Rule("<ColumnConstraint> ::= DEFAULT '(' <Expression> ')'", ConstructorParameterMapping = new[] {2})]
		public ColumnDefaultConstraint(Expression defaultValue) {}
	}
}