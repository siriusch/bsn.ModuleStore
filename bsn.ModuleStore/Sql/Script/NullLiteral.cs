using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class NullLiteral: Literal {
		[Rule("<NullLiteral> ::= NULL", AllowTruncationForConstructor = true)]
		public NullLiteral() {}
	}
}