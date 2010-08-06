using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CollableValue: Expression {
		private readonly Expression valueExpression;
		private readonly CollationName collation;

		[Rule("<CollatedValue> ::= <Value> COLLATE <CollationName>", ConstructorParameterMapping = new[] {0, 2})]
		public CollableValue(Expression valueExpression, CollationName collation) {
			if (valueExpression == null) {
				throw new ArgumentNullException("valueExpression");
			}
			if (collation == null) {
				throw new ArgumentNullException("collation");
			}
			this.valueExpression = valueExpression;
			this.collation = collation;
		}
	}
}