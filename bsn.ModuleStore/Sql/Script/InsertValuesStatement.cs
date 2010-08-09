using System;
using System.Collections.Generic;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class InsertValuesStatement: InsertStatement {
		private readonly List<ColumnName> columnNames;
		private readonly OutputClause output;

		protected InsertValuesStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, Optional<Sequence<ColumnName>> columnNames, OutputClause output): base(ctes, topExpression, destinationRowset) {
			if (output == null) {
				throw new ArgumentNullException("output");
			}
			this.columnNames = columnNames.ToList();
			this.output = output;
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			base.WriteTo(writer);
			if (columnNames.Count > 0) {
				writer.Write(" (");
				writer.WriteSequence(columnNames, null, ", ", null);
				writer.Write(')');
			}
			writer.WriteScript(output, " ", null);
			writer.WriteLine();
		}
	}
}