using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class EnableDisableTriggerStatement: Statement {
		private readonly TableName tableName;
		private readonly List<TriggerName> triggerNames;

		protected EnableDisableTriggerStatement(Sequence<TriggerName> triggerNames, TableName tableName) {
			Debug.Assert(tableName != null);
			this.tableName = tableName;
			this.triggerNames = triggerNames.ToList();
		}

		public bool All {
			get {
				return triggerNames.Count == 0;
			}
		}

		public TableName TableName {
			get {
				return tableName;
			}
		}

		public List<TriggerName> TriggerNames {
			get {
				return triggerNames;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write(" TRIGGER ");
			if (All) {
				writer.Write("ALL");
			} else {
				writer.WriteSequence(triggerNames, WhitespacePadding.None, ", ");
			}
			writer.Write(" ON ");
			writer.WriteScript(tableName, WhitespacePadding.None);
		}
	}
}