using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DisableTriggerStatement: EnableDisableTriggerStatement {
		[Rule("<DisableTriggerStatement> ::= DISABLE_TRIGGER ALL ON <TriggerTarget>", ConstructorParameterMapping = new[] {3})]
		public DisableTriggerStatement(TriggerTarget target): this(null, target) {}

		[Rule("<DisableTriggerStatement> ::= DISABLE_TRIGGER <TriggerNameQualifiedList> ON <TriggerTarget>", ConstructorParameterMapping=new[] { 1, 3 })]
		public DisableTriggerStatement(Sequence<Qualified<SchemaName, TriggerName>> triggerNames, TriggerTarget target): base(triggerNames, target) {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("DISABLE");
			base.WriteTo(writer);
		}
	}
}