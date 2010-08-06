using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("NONCLUSTERED")]
	public class ConstraintNonclusteredToken: ConstraintClusterToken {
		public override Clustered Clustered {
			get {
				return Clustered.Nonclustered;
			}
		}
	}
}