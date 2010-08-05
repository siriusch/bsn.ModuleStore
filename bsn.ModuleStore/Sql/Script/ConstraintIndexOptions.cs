using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ConstraintIndexOptions: ConstraintIndex {
		[Rule("<ConstraintIndex> ::= <IndexOptionGroup>")]
		public ConstraintIndexOptions(Optional<Sequence<IndexOption>> indexOptions) {}
	}
}