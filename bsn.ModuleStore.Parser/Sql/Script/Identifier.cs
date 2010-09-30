using System;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("Id")]
	public class Identifier: SqlIdentifier {
		public Identifier(string id): base(id) {}

		internal Identifier(string id, Symbol symbol, LineInfo lineInfo): base(id) {
			base.Initialize(symbol, lineInfo);
		}
	}
}