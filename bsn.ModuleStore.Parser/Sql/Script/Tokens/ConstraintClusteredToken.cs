using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("CLUSTERED")]
	public class ConstraintClusteredToken: ConstraintClusterToken {
		public override Clustered Clustered {
			get {
				return Clustered.Clustered;
			}
		}
	}
}