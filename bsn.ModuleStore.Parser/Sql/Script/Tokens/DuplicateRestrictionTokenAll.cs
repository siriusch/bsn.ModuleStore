using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("ALL")]
	public sealed class DuplicateRestrictionTokenAll: DuplicateRestrictionToken {
		public override bool Distinct {
			get {
				return false;
			}
		}
	}
}