using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class InsertSelectValuesStatement: InsertValuesStatement {
		private readonly SelectQuery selectQuery;

		[Rule("<InsertStatement> ::= <QueryOptions> ~INSERT <OptionalTop> ~<OptionalInto> <DestinationRowset> <ColumnNameGroup> <OutputClause> <SelectQuery> <QueryHint>")]
		public InsertSelectValuesStatement(QueryOptions queryOptions, TopExpression topExpression, DestinationRowset destinationRowset, Optional<Sequence<ColumnName>> columnNames, OutputClause output, SelectQuery selectQuery, QueryHint queryHint)
				: base(queryOptions, topExpression, destinationRowset, columnNames, output, queryHint) {
			Debug.Assert(selectQuery != null);
			this.selectQuery = selectQuery;
		}

		public SelectQuery SelectQuery {
			get {
				return selectQuery;
			}
		}

		protected override void WriteToInternal(SqlWriter writer) {
			base.WriteToInternal(writer);
			writer.WriteScript(selectQuery, WhitespacePadding.NewlineBefore);
		}
	}
}