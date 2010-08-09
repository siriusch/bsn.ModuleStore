﻿using System;
using System.Collections.Generic;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class InsertExpressionValuesStatement: InsertValuesStatement {
		private readonly List<Expression> expressions;

		[Rule("<InsertStatement> ::= <CTEGroup> INSERT <Top> <OptionalInto> <DestinationRowset> <ColumnNameGroup> <OutputClause> VALUES '(' <ExpressionList> ')'", ConstructorParameterMapping = new[] {0, 2, 4, 5, 6, 9})]
		public InsertExpressionValuesStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, Optional<Sequence<ColumnName>> columnNames, OutputClause output, Sequence<Expression> expressions)
				: base(ctes, topExpression, destinationRowset, columnNames, output) {
			this.expressions = expressions.ToList();
		}

		public List<Expression> Expressions {
			get {
				return expressions;
			}
		}

		public override void WriteTo(TextWriter writer) {
			base.WriteTo(writer);
			writer.Write("VALUES (");
			writer.WriteSequence(expressions, null, ", ", null);
			writer.Write(')');
		}
	}
}