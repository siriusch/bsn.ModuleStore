using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class DuplicateRestrictionTokenAll: DuplicateRestrictionToken {
		[Rule("<Restriction> ::= ALL", AllowTruncationForConstructor = true)]
		public DuplicateRestrictionTokenAll() {}

		public override bool? Distinct {
			get {
				return false;
			}
		}
	}
}