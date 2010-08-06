using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public class ConstraintClusterToken: SqlToken {
		[Rule("<ConstraintCluster> ::=")]
		public ConstraintClusterToken(): base() {}

		public virtual Clustered Clustered {
			get {
				return Clustered.Undefined;
			}
		}
	}
}