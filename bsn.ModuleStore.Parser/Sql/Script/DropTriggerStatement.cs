using System;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DropTriggerStatement: DropStatement {
		private readonly Qualified<SchemaName, TriggerName> triggerName;

		[Rule("<DropTriggerStatement> ::= DROP TRIGGER <TriggerNameQualified>", ConstructorParameterMapping = new[] {2})]
		public DropTriggerStatement(Qualified<SchemaName, TriggerName> triggerName) {
			Debug.Assert(triggerName != null);
			this.triggerName = triggerName;
		}

		public Qualified<SchemaName, TriggerName> TriggerName {
			get {
				return triggerName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("DROP TRIGGER ");
			writer.WriteScript(triggerName, WhitespacePadding.None);
		}
	}
}