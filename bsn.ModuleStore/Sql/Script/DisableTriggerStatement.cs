using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class DisableTriggerStatement: EnableDisableTriggerStatement {
		[Rule("<DisableTriggerStatement> ::= DISABLE_TRIGGER ALL ON <TableName>", ConstructorParameterMapping=new[] { 3 })]
		public DisableTriggerStatement(TableName tableName)
				: this(null, tableName) {
		}

		[Rule("<DisableTriggerStatement> ::= DISABLE_TRIGGER <TriggerNameList> ON <TableName>", ConstructorParameterMapping=new[] { 1, 3 })]
		public DisableTriggerStatement(Sequence<TriggerName> triggerNames, TableName tableName)
				: base(triggerNames, tableName) {
		}
	}
}