using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("FOR_XML_RAW")]
	public sealed class ForXmlRawToken: ForXmlToken {
		public override ForXmlKind Kind {
			get {
				return ForXmlKind.Raw;
			}
		}
	}
}