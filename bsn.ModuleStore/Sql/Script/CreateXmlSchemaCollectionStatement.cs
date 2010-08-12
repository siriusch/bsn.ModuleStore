using System;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateXmlSchemaCollectionStatement: CreateStatement {
		private readonly Expression expression;
		private readonly XmlSchemaCollectionName xmlSchemaCollectionName;

		[Rule("<CreateXmlSchemaCollectionStatement> ::= CREATE XML_SCHEMA_COLLECTION <XmlSchemaCollectionName> AS <Expression>", ConstructorParameterMapping = new[] {2, 4})]
		public CreateXmlSchemaCollectionStatement(XmlSchemaCollectionName xmlSchemaCollectionName, Expression expression) {
			Debug.Assert(xmlSchemaCollectionName != null);
			Debug.Assert(expression != null);
			this.xmlSchemaCollectionName = xmlSchemaCollectionName;
			this.expression = expression;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public XmlSchemaCollectionName XmlSchemaCollectionName {
			get {
				return xmlSchemaCollectionName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CERATE XML SCHEMA COLLECTION ");
			writer.WriteScript(xmlSchemaCollectionName, WhitespacePadding.None);
			writer.Write(" AS ");
			writer.WriteScript(expression, WhitespacePadding.None);
		}
	}
}