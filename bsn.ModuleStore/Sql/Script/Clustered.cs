using System;
using System.Linq;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class Clustered: SqlToken {
		private readonly bool? clustered;

		[Rule("<ConstraintCluster> ::=")]
		public Clustered(): this(null) {}

		[Rule("<ConstraintCluster> ::= CLUSTERED")]
		[Rule("<ConstraintCluster> ::= NONCLUSTERED")]
		private Clustered(IToken token) {
			if (token != null) {
				clustered = token.NameIs("CLUSTERED");
			}
		}
	}
}