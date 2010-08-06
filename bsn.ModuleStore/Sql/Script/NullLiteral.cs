using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class NullLiteral: Literal {
		[Rule("<NullLiteral> ::= NULL", AllowTruncationForConstructor = true)]
		public NullLiteral() {}
	}
}
