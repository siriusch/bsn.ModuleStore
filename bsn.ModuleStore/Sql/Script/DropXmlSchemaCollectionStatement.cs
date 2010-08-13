using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DropXmlSchemaCollectionStatement: DropStatement {
		private readonly Qualified<SchemaName, XmlSchemaCollectionName> xmlSchemaCollectionName;

		[Rule("<DropXmlSchemaCollectionStatement> ::= DROP XML_SCHEMA_COLLECTION <XmlSchemaCollectionNameQualified>", ConstructorParameterMapping = new[] {2})]
		public DropXmlSchemaCollectionStatement(Qualified<SchemaName, XmlSchemaCollectionName> xmlSchemaCollectionName) {
			Debug.Assert(xmlSchemaCollectionName != null);
			this.xmlSchemaCollectionName = xmlSchemaCollectionName;
		}

		public Qualified<SchemaName, XmlSchemaCollectionName> XmlSchemaCollectionName {
			get {
				return xmlSchemaCollectionName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("DROP XML SCHEMA COLLECTION ");
			writer.WriteScript(xmlSchemaCollectionName, WhitespacePadding.None);
		}
	}
}