using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class EnableTriggerStatement: EnableDisableTriggerStatement {
		[Rule("<EnableTriggerStatement> ::= ENABLE_TRIGGER ALL ON <TriggerTarget>", ConstructorParameterMapping = new[] {3})]
		public EnableTriggerStatement(TriggerTarget target): this(null, target) {}

		[Rule("<EnableTriggerStatement> ::= ENABLE_TRIGGER <TriggerNameQualifiedList> ON <TriggerTarget>", ConstructorParameterMapping = new[] {1, 3})]
		public EnableTriggerStatement(Sequence<Qualified<SchemaName, TriggerName>> triggerNames, TriggerTarget target): base(triggerNames, target) {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("ENABLE");
			base.WriteTo(writer);
		}
	}
}