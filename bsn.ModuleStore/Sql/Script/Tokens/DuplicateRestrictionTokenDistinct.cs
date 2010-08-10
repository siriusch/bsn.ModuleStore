using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public sealed class DuplicateRestrictionTokenDistinct: DuplicateRestrictionToken {
		[Rule("<Restriction> ::= DISTINCT")]
		public DuplicateRestrictionTokenDistinct() {}

		public override bool? Distinct {
			get {
				return true;
			}
		}
	}
}