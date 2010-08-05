using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class InsertStatement: SqlStatement {
		protected InsertStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset) {}
	}
}