using System;
using System.Collections.Generic;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ConstraintIndexOptions: ConstraintIndex {
		private readonly List<IndexOption> indexOptions;

		[Rule("<ConstraintIndex> ::= <IndexOptionGroup>")]
		public ConstraintIndexOptions(Optional<Sequence<IndexOption>> indexOptions) {
			this.indexOptions = indexOptions.ToList();
		}

		public override bool HasValue {
			get {
				return indexOptions.Count > 0;
			}
		}

		public List<IndexOption> IndexOptions {
			get {
				return indexOptions;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteIndexOptions(indexOptions);
		}
	}
}