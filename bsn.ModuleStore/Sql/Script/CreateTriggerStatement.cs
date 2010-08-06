using System;
using System.Collections.Generic;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class CreateTriggerStatement: SqlCreateStatement {
		private readonly bool notForReplication;
		private readonly SqlStatement statement;
		private readonly TableName tableName;
		private readonly TriggerName triggerName;
		private readonly List<TriggerOperation> triggerOperations;
		private readonly TriggerType type;

		[Rule("<CreateTriggerStatement> ::= CREATE TRIGGER <TriggerName> ON <TableName> <TriggerType> <TriggerOperationList> <OptionalNotForReplication> AS <StatementGroup>", ConstructorParameterMapping = new[] {2, 4, 5, 6, 7, 9})]
		public CreateTriggerStatement(TriggerName triggerName, TableName tableName, TriggerTypeToken triggerType, Sequence<TriggerOperation> triggerOperations, Optional<ForReplicationToken> notForReplication, SqlStatement statement) {
			if (triggerName == null) {
				throw new ArgumentNullException("triggerName");
			}
			if (tableName == null) {
				throw new ArgumentNullException("tableName");
			}
			if (triggerType == null) {
				throw new ArgumentNullException("triggerType");
			}
			if (statement == null) {
				throw new ArgumentNullException("statement");
			}
			this.triggerName = triggerName;
			this.tableName = tableName;
			this.statement = statement;
			this.triggerOperations = triggerOperations.ToList();
			this.notForReplication = notForReplication.HasValue();
			type = triggerType.TriggerType;
		}
	}
}