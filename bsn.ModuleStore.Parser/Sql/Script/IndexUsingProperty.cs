using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class IndexUsingProperty: IndexUsing {
		[Rule("<IndexUsing> ::= ~USING ~XML ~INDEX <IndexName> ~FOR ~PROPERTY")]
		public IndexUsingProperty(IndexName indexName): base(indexName) {}

		public override IndexFor IndexFor {
			get {
				return IndexFor.Property;
			}
		}

		protected override string IndexForSpecifier {
			get {
				return "PROPERTY";
			}
		}
	}
}