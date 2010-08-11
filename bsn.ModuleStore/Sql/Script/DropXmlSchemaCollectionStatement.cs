using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DropXmlSchemaCollectionStatement: DropStatement {
		private readonly XmlSchemaCollectionName xmlSchemaCollectionName;

		[Rule("<DropXmlSchemaCollectionStatement> ::= DROP XML_SCHEMA_COLLECTION <XmlSchemaCollectionName>", ConstructorParameterMapping = new[] {2})]
		public DropXmlSchemaCollectionStatement(XmlSchemaCollectionName xmlSchemaCollectionName) {
			Debug.Assert(xmlSchemaCollectionName != null);
			this.xmlSchemaCollectionName = xmlSchemaCollectionName;
		}

		public XmlSchemaCollectionName XmlSchemaCollectionName {
			get {
				return xmlSchemaCollectionName;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("DROP XML SCHEMA COLLECTION ");
			writer.WriteScript(xmlSchemaCollectionName);
		}
	}
}