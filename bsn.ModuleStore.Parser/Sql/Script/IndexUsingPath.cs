using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class IndexUsingPath: IndexUsing {
		[Rule("<IndexUsing> ::= ~USING ~XML ~INDEX <IndexName> ~FOR ~PATH")]
		public IndexUsingPath(IndexName indexName): base(indexName) {}

		public override IndexFor IndexFor {
			get {
				return IndexFor.Path;
			}
		}

		protected override string IndexForSpecifier {
			get {
				return "PATH";
			}
		}
	}
}