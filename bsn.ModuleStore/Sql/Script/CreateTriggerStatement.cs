using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateTriggerStatement: SqlCreateStatement {
		[Rule("<CreateTriggerStatement> ::= CREATE TRIGGER <TriggerName> ON <TableName> <TriggerType> <TriggerOperationList> <OptionalNotForReplication> AS <StatementGroup>", ConstructorParameterMapping = new[] {2, 4, 5, 6, 7, 9})]
		public CreateTriggerStatement(TriggerName triggerName, TableName tableName, SqlToken triggerType, Sequence<TriggerOperation> triggerOperations, Optional<ForReplication> notForReplication, SqlStatement statement) {}
	}
}