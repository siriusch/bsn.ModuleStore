using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("DISTINCT")]
	public sealed class DuplicateRestrictionDistinctToken: DuplicateRestrictionToken {
		public DuplicateRestrictionDistinctToken() {}

		public override bool Distinct {
			get {
				return true;
			}
		}
	}
}