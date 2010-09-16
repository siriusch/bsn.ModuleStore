using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateXmlSchemaCollectionStatement: CreateStatement {
		private readonly Expression expression;
		private readonly Qualified<SchemaName, XmlSchemaCollectionName> xmlSchemaCollectionName;

		[Rule("<CreateXmlSchemaCollectionStatement> ::= ~CREATE ~XML_SCHEMA_COLLECTION <XmlSchemaCollectionNameQualified> ~AS <Expression>")]
		public CreateXmlSchemaCollectionStatement(Qualified<SchemaName, XmlSchemaCollectionName> xmlSchemaCollectionName, Expression expression) {
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

		public override ObjectCategory ObjectCategory {
			get {
				return ObjectCategory.XmlSchema;
			}
		}

		public override string ObjectName {
			get {
				return xmlSchemaCollectionName.Name.Value;
			}
		}

		public Qualified<SchemaName, XmlSchemaCollectionName> XmlSchemaCollectionName {
			get {
				return xmlSchemaCollectionName;
			}
		}

		public override DropStatement CreateDropStatement() {
			return new DropXmlSchemaCollectionStatement(xmlSchemaCollectionName);
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CERATE XML SCHEMA COLLECTION ");
			writer.WriteScript(xmlSchemaCollectionName, WhitespacePadding.None);
			writer.Write(" AS ");
			writer.WriteScript(expression, WhitespacePadding.None);
		}

		protected override string GetObjectSchema() {
			return xmlSchemaCollectionName.IsQualified ? xmlSchemaCollectionName.Qualification.Value : string.Empty;
		}
	}
}
