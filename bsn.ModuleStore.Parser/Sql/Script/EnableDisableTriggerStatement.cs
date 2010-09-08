using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class EnableDisableTriggerStatement: Statement {
		private readonly TriggerTarget target;
		private readonly List<Qualified<SchemaName, TriggerName>> triggerNames;

		protected EnableDisableTriggerStatement(Sequence<Qualified<SchemaName, TriggerName>> triggerNames, TriggerTarget target) {
			Debug.Assert(target != null);
			this.target = target;
			this.triggerNames = triggerNames.ToList();
		}

		public bool All {
			get {
				return triggerNames.Count == 0;
			}
		}

		public TriggerTarget Target {
			get {
				return target;
			}
		}

		public IEnumerable<Qualified<SchemaName, TriggerName>> TriggerNames {
			get {
				return triggerNames;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write(" TRIGGER ");
			if (All) {
				writer.Write("ALL");
			} else {
				writer.WriteScriptSequence(triggerNames, WhitespacePadding.None, ", ");
			}
			writer.Write(" ON ");
			writer.WriteScript(target, WhitespacePadding.None);
		}
	}
}