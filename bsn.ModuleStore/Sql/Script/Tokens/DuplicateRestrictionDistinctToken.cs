using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class DuplicateRestrictionDistinctToken: DuplicateRestrictionToken {
		[Rule("<Restriction> ::= DISTINCT", AllowTruncationForConstructor = true)]
		public DuplicateRestrictionDistinctToken() {}

		public override bool? Distinct {
			get {
				return true;
			}
		}
	}
}