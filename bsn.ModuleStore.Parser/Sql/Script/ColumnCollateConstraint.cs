using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnCollateConstraint: ColumnConstraint {
		private readonly CollationName collation;

		[Rule("<ColumnConstraint> ::= ~COLLATE <CollationName>")]
		public ColumnCollateConstraint(CollationName collation) {
			Debug.Assert(collation != null);
			this.collation = collation;
		}

		public CollationName Collation {
			get {
				return collation;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("COLLATE ");
			writer.WriteScript(collation, WhitespacePadding.None);
		}
	}
}