using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class XmlElementName: SqlQuotedName {
		[Rule("<XmlElementName> ::= Id")]
		public XmlElementName(Identifier identifier): base(identifier.Value) {}
	}
}