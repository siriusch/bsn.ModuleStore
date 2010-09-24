using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class InsertExecuteValuesStatement: InsertValuesStatement {
		private readonly ExecuteStatement executeStatement;

		[Rule("<InsertStatement> ::= <CTEGroup> ~INSERT <OptionalTop> ~<OptionalInto> <DestinationRowset> <ColumnNameGroup> <OutputClause> <ExecuteStatement> <QueryHint>")]
		public InsertExecuteValuesStatement(QueryOptions queryOptions, TopExpression topExpression, DestinationRowset destinationRowset, Optional<Sequence<ColumnName>> columnNames, OutputClause output, ExecuteStatement executeStatement, QueryHint queryHint)
				: base(queryOptions, topExpression, destinationRowset, columnNames, output, queryHint) {
			Debug.Assert(executeStatement != null);
			this.executeStatement = executeStatement;
		}

		public ExecuteStatement ExecuteStatement {
			get {
				return executeStatement;
			}
		}

		protected override void WriteToInternal(SqlWriter writer) {
			base.WriteToInternal(writer);
			writer.WriteScript(executeStatement, WhitespacePadding.None);
		}
	}
}