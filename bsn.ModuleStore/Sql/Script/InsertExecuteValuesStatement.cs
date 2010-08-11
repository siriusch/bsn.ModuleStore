using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class InsertExecuteValuesStatement: InsertValuesStatement {
		private readonly ExecuteStatement executeStatement;

		[Rule("<InsertStatement> ::= <CTEGroup> INSERT <OptionalTop> <OptionalInto> <DestinationRowset> <ColumnNameGroup> <OutputClause> <ExecuteStatement> <QueryHint>", ConstructorParameterMapping = new[] {0, 2, 4, 5, 6, 7, 8})]
		public InsertExecuteValuesStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, Optional<Sequence<ColumnName>> columnNames, OutputClause output, ExecuteStatement executeStatement, QueryHint queryHint)
				: base(ctes, topExpression, destinationRowset, columnNames, output, queryHint) {
			Debug.Assert(executeStatement != null);
			this.executeStatement = executeStatement;
		}

		public ExecuteStatement ExecuteStatement {
			get {
				return executeStatement;
			}
		}

		public override void WriteTo(TextWriter writer) {
			base.WriteTo(writer);
			writer.WriteScript(executeStatement, null, null);
			writer.WriteScript(QueryHint, " ", null);
		}
	}
}