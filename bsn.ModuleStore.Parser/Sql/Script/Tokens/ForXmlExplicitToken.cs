using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("FOR_XML_EXPLICIT")]
	public sealed class ForXmlExplicitToken: ForXmlToken {
		public override ForXmlKind Kind {
			get {
				return ForXmlKind.Explicit;
			}
		}
	}
}