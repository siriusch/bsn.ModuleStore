using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnCollateConstraint: ColumnConstraint {
		private readonly CollationName collation;

		[Rule("<ColumnConstraint> ::= COLLATE <CollationName>", ConstructorParameterMapping = new[] {1})]
		public ColumnCollateConstraint(CollationName collation) {
			if (collation == null) {
				throw new ArgumentNullException("collation");
			}
			this.collation = collation;
		}

		public CollationName Collation {
			get {
				return collation;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("COLLATE ");
			writer.WriteScript(collation);
		}
	}
}