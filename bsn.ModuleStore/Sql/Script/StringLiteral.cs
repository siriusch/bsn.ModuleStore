using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("StringLiteral")]
	public class StringLiteral: Literal<string> {
		public StringLiteral(string value): base(value) {
			if (value == null) {
				throw new ArgumentNullException("value");
			}
		}
	}
}