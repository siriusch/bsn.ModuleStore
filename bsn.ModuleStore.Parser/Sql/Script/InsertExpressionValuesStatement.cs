using System;
using System.Collections.Generic;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class InsertExpressionValuesStatement: InsertValuesStatement {
		private readonly List<Expression> expressions;

		[Rule("<InsertStatement> ::= <CTEGroup> ~INSERT <OptionalTop> ~<OptionalInto> <DestinationRowset> <ColumnNameGroup> <OutputClause> ~VALUES ~'(' <ExpressionList> ~')' <QueryHint>")]
		public InsertExpressionValuesStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, Optional<Sequence<ColumnName>> columnNames, OutputClause output, Sequence<Expression> expressions, QueryHint queryHint)
				: base(ctes, topExpression, destinationRowset, columnNames, output, queryHint) {
			this.expressions = expressions.ToList();
		}

		public IEnumerable<Expression> Expressions {
			get {
				return expressions;
			}
		}

		protected override void WriteToInternal(SqlWriter writer) {
			base.WriteToInternal(writer);
			writer.Write("VALUES (");
			writer.WriteScriptSequence(expressions, WhitespacePadding.None, ", ");
			writer.Write(')');
		}
	}
}