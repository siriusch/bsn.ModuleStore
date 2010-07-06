using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class XmlSchemaCollectionName: SqlQuotedName {
		[Rule("<XmlSchemaCollectionName> ::= Id")]
		public XmlSchemaCollectionName(Identifier identifier): base(identifier.Value) {}
	}
}