﻿using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CaseWhen<T>: SqlScriptableToken where T: SqlComputable {
		private readonly T condition;
		private readonly Expression valueExpression;

		[Rule("<CaseWhenExpression> ::= ~WHEN <Expression> ~THEN <Expression>", typeof(Expression))]
		[Rule("<CaseWhenPredicate> ::= ~WHEN <Predicate> ~THEN <Expression>", typeof(Predicate))]
		public CaseWhen(T condition, Expression valueExpression) {
			this.condition = condition;
			this.valueExpression = valueExpression;
		}

		public T Condition {
			get {
				return condition;
			}
		}

		public Expression ValueExpression {
			get {
				return valueExpression;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("WHEN ");
			writer.IncreaseIndent();
			writer.WriteScript(condition, WhitespacePadding.None);
			writer.DecreaseIndent();
			writer.Write(" THEN ");
			writer.IncreaseIndent();
			writer.WriteScript(valueExpression, WhitespacePadding.None);
			writer.DecreaseIndent();
		}
	}
}