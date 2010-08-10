using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("FOR_XML_AUTO")]
	public sealed class ForXmlAutoToken: ForXmlToken {
		public override ForXmlKind Kind {
			get {
				return ForXmlKind.Auto;
			}
		}
	}
}