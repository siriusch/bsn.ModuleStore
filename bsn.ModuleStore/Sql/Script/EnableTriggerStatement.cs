using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class EnableTriggerStatement: EnableDisableTriggerStatement {
		[Rule("<EnableTriggerStatement> ::= ENABLE_TRIGGER ALL ON <TableName>", ConstructorParameterMapping=new[] { 3 })]
		public EnableTriggerStatement(TableName tableName) : this(null, tableName) {
		}

		[Rule("<EnableTriggerStatement> ::= ENABLE_TRIGGER <TriggerNameList> ON <TableName>", ConstructorParameterMapping=new[] { 1, 3 })]
		public EnableTriggerStatement(Sequence<TriggerName> triggerNames, TableName tableName): base(triggerNames, tableName) {}
	}
}
