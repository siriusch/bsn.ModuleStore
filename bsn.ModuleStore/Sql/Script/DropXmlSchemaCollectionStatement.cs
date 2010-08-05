using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class DropXmlSchemaCollectionStatement: SqlDropStatement {
		[Rule("<DropXmlSchemaCollectionStatement> ::= DROP XML_SCHEMA_COLLECTION <XmlSchemaCollectionName>", ConstructorParameterMapping = new[] {2})]
		public DropXmlSchemaCollectionStatement(XmlSchemaCollectionName xmlSchemaCollectionName) {}
	}
}