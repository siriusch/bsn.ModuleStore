using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CollationName: SqlName {
		[Rule("<CollationName> ::= Id")]
		public CollationName(Identifier identifier): this(identifier.Value) {}

		internal CollationName(string name): base(name) {}
	}
}