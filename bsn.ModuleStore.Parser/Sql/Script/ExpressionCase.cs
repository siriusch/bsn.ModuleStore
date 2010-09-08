using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class ExpressionCase<T>: Expression where T: SqlComputable {
		private readonly Expression elseExpression;
		private readonly List<CaseWhen<T>> whenItems;

		protected ExpressionCase(Sequence<CaseWhen<T>> whenItems, Expression elseExpression) {
			Debug.Assert(whenItems != null);
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

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScriptSequence(whenItems, WhitespacePadding.NewlineBefore, null);
			if (elseExpression != null) {
				writer.WriteLine();
				writer.Write("ELSE ");
				writer.IncreaseIndent();
				writer.WriteScript(elseExpression, WhitespacePadding.None);
				writer.DecreaseIndent();
			}
			writer.WriteLine();
			writer.Write("END");
		}
	}
}