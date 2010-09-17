using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionSearchedCase: ExpressionCase<Predicate> {
		[Rule("<ExpressionCase> ::= ~CASE <CaseWhenPredicateList> ~END")]
		public ExpressionSearchedCase(Sequence<CaseWhen<Predicate>> whenItems): this(whenItems, null) {}

		[Rule("<ExpressionCase> ::= ~CASE <CaseWhenPredicateList> ~ELSE <Expression> ~END")]
		public ExpressionSearchedCase(Sequence<CaseWhen<Predicate>> whenItems, Expression elseExpression): base(whenItems, elseExpression) {}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("CASE");
			base.WriteTo(writer);
		}
	}
}