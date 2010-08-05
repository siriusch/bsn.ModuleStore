using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateXmlSchemaCollectionStatement: SqlCreateStatement {
		private readonly Expression expression;
		private readonly XmlSchemaCollectionName xmlSchemaCollectionName;

		[Rule("<CreateXmlSchemaCollectionStatement> ::= CREATE XML_SCHEMA_COLLECTION <XmlSchemaCollectionName> AS <Expression>", ConstructorParameterMapping = new[] {2, 4})]
		public CreateXmlSchemaCollectionStatement(XmlSchemaCollectionName xmlSchemaCollectionName, Expression expression) {
			if (xmlSchemaCollectionName == null) {
				throw new ArgumentNullException("xmlSchemaCollectionName");
			}
			if (expression == null) {
				throw new ArgumentNullException("expression");
			}
			this.xmlSchemaCollectionName = xmlSchemaCollectionName;
			this.expression = expression;
		}
	}
}