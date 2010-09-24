using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class InsertValuesStatement: InsertStatement {
		private readonly List<ColumnName> columnNames;
		private readonly OutputClause output;

		protected InsertValuesStatement(QueryOptions queryOptions, TopExpression topExpression, DestinationRowset destinationRowset, Optional<Sequence<ColumnName>> columnNames, OutputClause output, QueryHint queryHint): base(queryOptions, topExpression, destinationRowset, queryHint) {
			Debug.Assert(output != null);
			this.columnNames = columnNames.ToList();
			this.output = output;
		}

		public IEnumerable<ColumnName> ColumnNames {
			get {
				return columnNames;
			}
		}

		public OutputClause Output {
			get {
				return output;
			}
		}

		protected override void WriteToInternal(SqlWriter writer) {
			if (columnNames.Count > 0) {
				writer.Write(" (");
				writer.WriteScriptSequence(columnNames, WhitespacePadding.None, ", ");
				writer.WriteLine(")");
			}
			writer.WriteScript(output, WhitespacePadding.NewlineBefore);
		}
	}
}