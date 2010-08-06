using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class ExpressionCase<T>: Expression where T: SqlComputable {
		private readonly Expression elseExpression;
		private readonly Sequence<CaseWhen<T>> whenItems;

		protected ExpressionCase(Sequence<CaseWhen<T>> whenItems, Expression elseExpression) {
			if (whenItems == null) {
				throw new ArgumentNullException("whenItems");
			}
			this.whenItems = whenItems;
			this.elseExpression = elseExpression;
		}
	}
}