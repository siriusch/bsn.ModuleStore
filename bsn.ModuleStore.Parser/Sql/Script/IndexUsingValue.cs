using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class IndexUsingValue: IndexUsing {
		[Rule("<IndexUsing> ::= ~USING ~XML ~INDEX <IndexName> ~FOR ~VALUE")]
		public IndexUsingValue(IndexName indexName): base(indexName) {}

		public override IndexFor IndexFor {
			get {
				return IndexFor.Value;
			}
		}

		protected override string IndexForSpecifier {
			get {
				return "VALUE";
			}
		}
	}
}