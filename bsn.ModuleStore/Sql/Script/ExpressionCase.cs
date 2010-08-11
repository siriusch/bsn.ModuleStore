using System;
using System.Collections.Generic;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class ExpressionCase<T>: Expression where T: SqlComputable {
		private readonly Expression elseExpression;
		private readonly List<CaseWhen<T>> whenItems;

		protected ExpressionCase(Sequence<CaseWhen<T>> whenItems, Expression elseExpression) {
			if (whenItems == null) {
				throw new ArgumentNullException("whenItems");
			}
			this.whenItems = whenItems.ToList();
			this.elseExpression = elseExpression;
		}

		public Expression ElseExpression {
			get {
				return elseExpression;
			}
		}

		public List<CaseWhen<T>> WhenItems {
			get {
				return whenItems;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteLine();
			writer.WriteSequence(whenItems, null, null, Environment.NewLine);
			writer.WriteScript(elseExpression, "ELSE ", Environment.NewLine);
			writer.Write("END");
		}
	}
}