using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class InsertValuesStatement: InsertStatement {
		protected InsertValuesStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, Optional<Sequence<ColumnName>> columnNames, OutputClause output): base(ctes, topExpression, destinationRowset) {}
	}
}