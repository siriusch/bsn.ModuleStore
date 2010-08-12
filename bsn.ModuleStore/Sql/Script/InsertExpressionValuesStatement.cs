using System;
using System.Collections.Generic;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class InsertExpressionValuesStatement: InsertValuesStatement {
		private readonly List<Expression> expressions;

		[Rule("<InsertStatement> ::= <CTEGroup> INSERT <OptionalTop> <OptionalInto> <DestinationRowset> <ColumnNameGroup> <OutputClause> VALUES '(' <ExpressionList> ')' <QueryHint>", ConstructorParameterMapping = new[] {0, 2, 4, 5, 6, 9, 11})]
		public InsertExpressionValuesStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, Optional<Sequence<ColumnName>> columnNames, OutputClause output, Sequence<Expression> expressions, QueryHint queryHint)
				: base(ctes, topExpression, destinationRowset, columnNames, output, queryHint) {
			this.expressions = expressions.ToList();
		}

		public List<Expression> Expressions {
			get {
				return expressions;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("VALUES (");
			writer.WriteSequence(expressions, WhitespacePadding.None, ", ");
			writer.Write(')');
			writer.WriteScript(QueryHint, WhitespacePadding.SpaceBefore);
		}
	}
}