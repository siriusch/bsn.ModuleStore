using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("FOR_XML_PATH")]
	public sealed class ForXmlPathToken: ForXmlToken {
		public override ForXmlKind Kind {
			get {
				return ForXmlKind.Path;
			}
		}
	}
}