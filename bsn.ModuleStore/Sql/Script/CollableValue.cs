using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CollableValue: Expression {
		private readonly CollationName collation;
		private readonly Expression valueExpression;

		[Rule("<CollatedValue> ::= <Value> COLLATE <CollationName>", ConstructorParameterMapping = new[] {0, 2})]
		public CollableValue(Expression valueExpression, CollationName collation) {
			Debug.Assert(valueExpression != null);
			Debug.Assert(collation != null);
			this.valueExpression = valueExpression;
			this.collation = collation;
		}

		public CollationName Collation {
			get {
				return collation;
			}
		}

		public Expression ValueExpression {
			get {
				return valueExpression;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(valueExpression);
			writer.Write(" COLLATE ");
			writer.WriteScript(collation);
		}
	}
}