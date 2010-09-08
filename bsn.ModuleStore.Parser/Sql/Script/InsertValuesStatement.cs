using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class InsertValuesStatement: InsertStatement {
		private readonly List<ColumnName> columnNames;
		private readonly OutputClause output;

		protected InsertValuesStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, Optional<Sequence<ColumnName>> columnNames, OutputClause output, QueryHint queryHint): base(ctes, topExpression, destinationRowset, queryHint) {
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

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			if (columnNames.Count > 0) {
				writer.Write(" (");
				writer.WriteScriptSequence(columnNames, WhitespacePadding.None, ", ");
				writer.Write(')');
			}
			writer.WriteScript(output, WhitespacePadding.SpaceBefore);
			writer.WriteLine();
		}
	}
}