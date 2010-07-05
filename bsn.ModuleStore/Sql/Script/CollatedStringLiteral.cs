using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CollatedStringLiteral: StringLiteral {
		private readonly CollationName collation;

		[Rule("<CollableStringLiteral> ::= <StringLiteral> <OptionalCollate>")]
		public CollatedStringLiteral(StringLiteral literal, CollationName collation): base(literal.Value) {
			this.collation = collation;
		}

		public CollationName Collation {
			get {
				return collation;
			}
		}
	}
}