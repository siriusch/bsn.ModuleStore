using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CollationName: SqlName {
		[Rule("<CollationName> ::= Id")]
		public CollationName(Identifier identifier): base(identifier.Value) {}
	}
}