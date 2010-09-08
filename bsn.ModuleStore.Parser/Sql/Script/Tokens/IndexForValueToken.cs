using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("FOR_VALUE")]
	public sealed class IndexForValueToken: IndexForToken {
		public override IndexFor IndexFor {
			get {
				return IndexFor.Value;
			}
		}
	}
}