using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class OpenxmlImplicitSchema: OpenxmlSchema {
		[Rule("<OpenxmlImplicitSchema> ::= WITH '(' <TableNameQualified> ')'", ConstructorParameterMapping = new[] {2})]
		public OpenxmlImplicitSchema(Qualified<TableName> tableName) {}
	}
}