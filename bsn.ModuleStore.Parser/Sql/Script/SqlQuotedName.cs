using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlQuotedName: SqlName {
		protected SqlQuotedName(string name): base(name) {}

		protected internal override void WriteToInternal(SqlWriter writer, bool isPartOfQualifiedName) {
			writer.WriteDelimitedIdentifier(Value);
		}
	}
}