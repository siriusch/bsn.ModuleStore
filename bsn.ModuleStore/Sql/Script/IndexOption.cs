using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class IndexOption: SqlToken {
		[Rule("<IndexOption> ::= Id '=' <IntegerLiteral>", ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<IndexOption> ::= Id '=' <Toggle>", ConstructorParameterMapping=new[] { 0, 2 })]
		public IndexOption(Identifier key, SqlToken value) {}
	}
}