using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class EnableDisableTriggerStatement: SqlStatement {
		private TableName tableName;
		private Sequence<TriggerName> triggerNames;

		protected EnableDisableTriggerStatement(Sequence<TriggerName> triggerNames, TableName tableName) {
			if (tableName == null) {
				throw new ArgumentNullException("tableName");
			}
			this.tableName = tableName;
			this.triggerNames = triggerNames;
		}
	}
}