using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class DropViewStatement: SqlDropStatement {
		private readonly ViewName viewName;

		[Rule("<DropViewStatement> ::= DROP VIEW <ViewName>", ConstructorParameterMapping = new[] {2})]
		public DropViewStatement(ViewName viewName) {
			this.viewName = viewName;
		}
	}
}