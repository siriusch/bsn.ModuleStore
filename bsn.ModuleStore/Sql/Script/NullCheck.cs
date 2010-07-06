using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class NullCheck: SqlToken {
		private readonly bool not;

		[Rule("<NullCheck> ::= NULL", AllowTruncationForConstructor = true)]
		public NullCheck(): this(null) {}

		[Rule("<NullCheck> ::= NOT NULL", AllowTruncationForConstructor = true)]
		public NullCheck(NotToken not) {
			this.not = not != null;
		}

		public bool Not {
			get {
				return not;
			}
		}
	}
}