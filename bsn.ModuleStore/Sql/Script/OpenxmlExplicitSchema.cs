using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class OpenxmlExplicitSchema: OpenxmlSchema {
		[Rule("<OpenxmlExplicitSchema> ::= WITH '(' <OpenxmlColumnList> ')'", ConstructorParameterMapping = new[] {2})]
		public OpenxmlExplicitSchema(Sequence<OpenxmlColumn> columns) {}
	}
}