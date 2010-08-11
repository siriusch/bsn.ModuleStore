using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PredicateNull: PredicateNegable {
		private readonly Expression valueExpression;

		[Rule("<PredicateNull> ::= <Expression> IS NULL", AllowTruncationForConstructor = true)]
		public PredicateNull(Expression valueExpression): this(valueExpression, false) {}

		protected PredicateNull(Expression valueExpression, bool not): base(not) {
			Debug.Assert(valueExpression != null);
			this.valueExpression = valueExpression;
		}

		public Expression ValueExpression {
			get {
				return valueExpression;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteScript(valueExpression);
			writer.Write(" IS");
			base.WriteTo(writer);
			writer.Write(" NULL");
		}
	}
}