using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DropTriggerStatement: DropStatement {
		private readonly TriggerName triggerName;

		[Rule("<DropTriggerStatement> ::= DROP TRIGGER <TriggerName>", ConstructorParameterMapping = new[] {2})]
		public DropTriggerStatement(TriggerName triggerName) {
			Debug.Assert(triggerName != null);
			this.triggerName = triggerName;
		}

		public TriggerName TriggerName {
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