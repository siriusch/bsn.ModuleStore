using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class Collable<T>: Expression where T: Expression {
		private readonly CollationName collation;
		private readonly T value;

		[Rule("<CollatedValue> ::= <Value> COLLATE <CollationName>", typeof(Expression), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<CollableStringLiteral> ::= <StringLiteral> COLLATE <CollationName>", typeof(StringLiteral), ConstructorParameterMapping = new[] {0, 2})]
		public Collable(T value, CollationName collation) {
			if (value == null) {
				throw new ArgumentNullException("value");
			}
			if (collation == null) {
				throw new ArgumentNullException("collation");
			}
			this.value = value;
			this.collation = collation;
		}
	}
}