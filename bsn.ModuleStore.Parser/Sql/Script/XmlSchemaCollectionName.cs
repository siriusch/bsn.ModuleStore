using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class XmlSchemaCollectionName: SqlQuotedName {
		[Rule("<XmlSchemaCollectionName> ::= Id")]
		[Rule("<XmlSchemaCollectionName> ::= QuotedId")]
		public XmlSchemaCollectionName(Identifier identifier): base(identifier.Value) {}
	}
}